using Common;
using Common.DTO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server;

public class GameServer
{
    private TcpListener listener = null!;
    private List<Player> players = new List<Player>();
    private int nextPlayerId = 1;
    private bool gameStarted = false;
    private int currentCycle = 0;
    private int currentTurnIndex = 0;
    private List<int> turnOrder = new List<int>();
    private int totalCycles = 15;
    private int currentTurn = 0;

    public void Start(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Server started on port " + port);

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");
            Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var msg = JsonSerializer.Deserialize<NetworkMessage>(json);

                if (msg.Type == MessageType.JOIN)
                {
                    var joinDto = JsonSerializer.Deserialize<JoinDto>(msg.Payload);
                    var player = new Player
                    {
                        Id = nextPlayerId++,
                        Nickname = joinDto.Nickname,
                        Email = joinDto.Email,
                        Client = client
                    };
                    players.Add(player);
                    Console.WriteLine($"Player {player.Nickname} joined");

                    SendResponse(player, true, null, "Joined successfully");

                    if (players.Count >= 2 && !gameStarted)
                    {
                        StartGame();
                    }
                }
                else
                {
                    var player = players.FirstOrDefault(p => p.Client == client);
                    if (player != null)
                    {
                        HandlePlayerMessage(player, msg);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        Console.WriteLine("Game starting...");

        foreach (var p in players)
        {
            var startDto = new StartGameDto
            {
                PlayerId = p.Id,
                PlayerCount = players.Count
            };
            SendMessage(p, MessageType.START_GAME, startDto);
        }
    }

    private void HandlePlayerMessage(Player player, NetworkMessage msg)
    {
        switch (msg.Type)
        {
            case MessageType.ARCHETYPE:
                var archetypeDto = JsonSerializer.Deserialize<ArchetypeDto>(msg.Payload);
                player.Archetype = archetypeDto.ArchetypeType;
                var startRes = GameLogic.GetStartResources();
                foreach (var r in startRes)
                {
                    player.AddResource(r.Key, r.Value);
                }
                Console.WriteLine($"Player {player.Nickname} chose {player.Archetype}");

                bool allReady = players.All(p => p.Archetype != ArchetypeType.Neutral);
                if (allReady)
                {
                    StartFirstTurn();
                }
                break;

            case MessageType.BUILD:
                var buildDto = JsonSerializer.Deserialize<BuildRequestDto>(msg.Payload);
                HandleBuild(player, buildDto);
                break;

            case MessageType.UPGRADE:
                var upgradeDto = JsonSerializer.Deserialize<UpgradeRequestDto>(msg.Payload);
                HandleUpgrade(player, upgradeDto);
                break;

            case MessageType.MAKE_SOLDIERS:
                var soldiersDto = JsonSerializer.Deserialize<MakeSoldiersRequestDto>(msg.Payload);
                HandleMakeSoldiers(player, soldiersDto);
                break;

            case MessageType.ATTACK:
                var attackDto = JsonSerializer.Deserialize<AttackRequestDto>(msg.Payload);
                HandleAttack(player, attackDto);
                break;

            case MessageType.END_TURN:
                HandleEndTurn(player);
                break;
        }
    }

    private void StartFirstTurn()
    {
        currentCycle = 1;
        currentTurn = 1;
        ShuffleTurnOrder();
        currentTurnIndex = 0;
        StartPlayerTurn();
    }

    private void ShuffleTurnOrder()
    {
        turnOrder.Clear();
        foreach (var p in players)
        {
            turnOrder.Add(p.Id);
        }
        Random rnd = new Random();
        turnOrder = turnOrder.OrderBy(x => rnd.Next()).ToList();
    }

    private void StartPlayerTurn()
    {
        int playerId = turnOrder[currentTurnIndex];
        var player = players.First(p => p.Id == playerId);

        DoProduction(player);
        DoProcessing(player);

        var turnDto = new StartTurnDto
        {
            PlayerId = playerId,
            Cycle = currentCycle,
            Turn = currentTurn
        };

        foreach (var p in players)
        {
            SendMessage(p, MessageType.START_TURN, turnDto);
        }

        var stateDto = GetPlayerState(player);
        SendMessage(player, MessageType.STATE, stateDto);
    }

    private void DoProduction(Player player)
    {
        var produced = new Dictionary<Resources, int>();
        foreach (var b in player.Buildings)
        {
            if (GameLogic.IsProducer(b.Type))
            {
                var res = GameLogic.GetProducerOutput(b.Type);
                int amount = GameLogic.GetProduction(b.Type, b.Level);
                player.AddResource(res, amount);
                if (!produced.ContainsKey(res))
                    produced[res] = 0;
                produced[res] += amount;
            }
        }

        var prodDto = new ProductionResultDto
        {
            ProducedResources = produced.ToDictionary(k => k.Key.ToString(), v => v.Value)
        };
        SendMessage(player, MessageType.PRODUCTION_RESULT, prodDto);
    }

    private void DoProcessing(Player player)
    {
        foreach (var b in player.Buildings)
        {
            if (GameLogic.IsProcessor(b.Type))
            {
                var input = GameLogic.GetProcessInput(b.Type);
                var output = GameLogic.GetProcessOutput(b.Type);
                int maxTimes = GameLogic.GetProduction(b.Type, b.Level);

                for (int i = 0; i < maxTimes; i++)
                {
                    if (player.HasResources(input))
                    {
                        player.SpendResources(input);
                        player.AddResource(output, 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void HandleBuild(Player player, BuildRequestDto dto)
    {
        if (player.Buildings.Any(b => b.PlaceId == dto.PlaceId))
        {
            SendResponse(player, false, ErrorCode.PlaceOccupied, "Place occupied");
            return;
        }

        var cost = GameLogic.GetBuildCost(dto.Type);
        if (!player.HasResources(cost))
        {
            SendResponse(player, false, ErrorCode.NotEnoughResources, "Not enough resources");
            return;
        }

        player.SpendResources(cost);

        var building = new Building
        {
            PlaceId = dto.PlaceId,
            Type = dto.Type,
            Level = 1,
            TurnBuilt = currentTurn
        };
        player.Buildings.Add(building);

        if (player.Archetype == ArchetypeType.Engineer)
        {
            Random rnd = new Random();
            int refundCount = rnd.Next(1, 3);
            var costList = cost.ToList();
            for (int i = 0; i < refundCount && i < costList.Count; i++)
            {
                var refund = costList[rnd.Next(costList.Count)];
                player.AddResource(refund.Key, 1);
            }
        }

        player.RecalculateDefense();
        SendResponse(player, true, null, "Built successfully");
        var stateDto = GetPlayerState(player);
        SendMessage(player, MessageType.STATE, stateDto);
    }

    private void HandleUpgrade(Player player, UpgradeRequestDto dto)
    {
        var building = player.Buildings.FirstOrDefault(b => b.PlaceId == dto.PlaceId);
        if (building == null)
        {
            SendResponse(player, false, ErrorCode.BuildingNotFound, "Building not found");
            return;
        }

        if (building.TurnBuilt == currentTurn || building.Level >= 3)
        {
            SendResponse(player, false, ErrorCode.UpgradeNotAllowed, "Upgrade not allowed");
            return;
        }

        var cost = GameLogic.GetUpgradeCost(building.Type, building.Level + 1);
        if (!player.HasResources(cost))
        {
            SendResponse(player, false, ErrorCode.NotEnoughResources, "Not enough resources");
            return;
        }

        player.SpendResources(cost);
        building.Level++;
        building.TurnBuilt = currentTurn;

        player.RecalculateDefense();
        SendResponse(player, true, null, "Upgraded successfully");
        var stateDto = GetPlayerState(player);
        SendMessage(player, MessageType.STATE, stateDto);
    }

    private void HandleMakeSoldiers(Player player, MakeSoldiersRequestDto dto)
    {
        var barracks = player.Buildings.FirstOrDefault(b => b.PlaceId == dto.BarracksId && b.Type == BuildingType.Barracks);
        if (barracks == null)
        {
            SendResponse(player, false, ErrorCode.BuildingNotFound, "Barracks not found");
            return;
        }

        int maxSoldiers = GameLogic.GetProduction(BuildingType.Barracks, barracks.Level);
        if (dto.Count > maxSoldiers)
        {
            SendResponse(player, false, ErrorCode.InvalidAction, "Too many soldiers");
            return;
        }

        var soldierCost = GameLogic.GetSoldierCost(player.Archetype);
        var totalCost = new Dictionary<Resources, int>();
        foreach (var c in soldierCost)
        {
            totalCost[c.Key] = c.Value * dto.Count;
        }

        if (!player.HasResources(totalCost))
        {
            SendResponse(player, false, ErrorCode.NotEnoughResources, "Not enough resources");
            return;
        }

        player.SpendResources(totalCost);
        player.Soldiers += dto.Count;

        SendResponse(player, true, null, "Soldiers created");
        var stateDto = GetPlayerState(player);
        SendMessage(player, MessageType.STATE, stateDto);
    }

    private void HandleAttack(Player player, AttackRequestDto dto)
    {
        if (currentCycle <= 5)
        {
            SendResponse(player, false, ErrorCode.AttackNotAllowed, "Attack not allowed in first 5 cycles");
            return;
        }

        var target = players.FirstOrDefault(p => p.Id == dto.ToPlayerId);
        if (target == null || target.Id == player.Id)
        {
            SendResponse(player, false, ErrorCode.InvalidTarget, "Invalid target");
            return;
        }

        if (player.Soldiers < dto.Soldiers)
        {
            SendResponse(player, false, ErrorCode.InvalidAction, "Not enough soldiers");
            return;
        }

        player.Soldiers -= dto.Soldiers;

        int defense = target.TotalDefense;
        if (player.Archetype == ArchetypeType.Warrior)
        {
            defense = (int)(defense * 0.8);
        }
        else if (player.Archetype == ArchetypeType.Recruit)
        {
            defense = (int)(defense * 1.2);
        }

        int lost = (int)Math.Ceiling(dto.Soldiers * (defense / 100.0));
        if (lost > dto.Soldiers) lost = dto.Soldiers;
        int survived = dto.Soldiers - lost;

        var stolen = new Dictionary<Resources, int>();
        Random rnd = new Random();

        int stealsPerSoldier = 1;
        if (player.Archetype == ArchetypeType.Glutton)
        {
            stealsPerSoldier = 2;
        }

        for (int i = 0; i < survived * stealsPerSoldier; i++)
        {
            var availableRes = target.Resources.Where(r => r.Key != Resources.Soldier && r.Value > 0).ToList();
            if (availableRes.Count > 0)
            {
                var randomRes = availableRes[rnd.Next(availableRes.Count)];
                target.Resources[randomRes.Key]--;
                if (!stolen.ContainsKey(randomRes.Key))
                    stolen[randomRes.Key] = 0;
                stolen[randomRes.Key]++;
            }
        }

        foreach (var s in stolen)
        {
            player.AddResource(s.Key, s.Value);
        }

        player.Soldiers += survived;

        var attackDto = new AttackTargetDto
        {
            ToPlayerId = dto.ToPlayerId,
            Sent = dto.Soldiers,
            Lost = lost,
            StolenResources = stolen.ToDictionary(k => k.Key.ToString(), v => v.Value)
        };
        SendMessage(player, MessageType.ATTACK_TARGET, attackDto);

        var stateDto = GetPlayerState(player);
        SendMessage(player, MessageType.STATE, stateDto);
    }

    private void HandleEndTurn(Player player)
    {
        currentTurnIndex++;
        currentTurn++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentCycle++;
            currentTurnIndex = 0;
            ShuffleTurnOrder();

            if (currentCycle > totalCycles)
            {
                EndGame();
                return;
            }
        }

        var turnEndDto = new TurnEndedDto
        {
            PlayerId = player.Id,
            NextPlayerId = turnOrder[currentTurnIndex]
        };

        foreach (var p in players)
        {
            SendMessage(p, MessageType.TURN_ENDED, turnEndDto);
        }

        StartPlayerTurn();
    }

    private void EndGame()
    {
        int maxPoints = 0;
        Player winner = null;

        foreach (var p in players)
        {
            int points = p.CalculatePoints();
            if (points > maxPoints)
            {
                maxPoints = points;
                winner = p;
            }
        }

        var endDto = new GameEndDto
        {
            WinnerPlayerId = winner.Id,
            Points = maxPoints
        };

        foreach (var p in players)
        {
            SendMessage(p, MessageType.GAME_END, endDto);
        }

        Console.WriteLine($"Game ended. Winner: {winner.Nickname} with {maxPoints} points");
    }

    private StateDto GetPlayerState(Player player)
    {
        var state = new StateDto
        {
            Resources = player.Resources.ToDictionary(k => k.Key.ToString(), v => v.Value),
            Soldiers = player.Soldiers,
            Defense = player.TotalDefense,
            Buildings = player.Buildings.Select(b => new BuildingStateDto
            {
                PlaceId = b.PlaceId,
                Type = b.Type,
                Level = b.Level
            }).ToList()
        };
        return state;
    }

    private void SendMessage(Player player, MessageType type, object payload)
    {
        try
        {
            var msg = new NetworkMessage
            {
                Type = type,
                Payload = JsonSerializer.Serialize(payload)
            };
            string json = JsonSerializer.Serialize(msg);
            byte[] data = Encoding.UTF8.GetBytes(json);
            player.Client.GetStream().Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending message: " + ex.Message);
        }
    }

    private void SendResponse(Player player, bool success, ErrorCode? errorCode, string message)
    {
        var response = new ResponseDto
        {
            Success = success,
            ErrorCode = errorCode,
            Message = message
        };
        SendMessage(player, MessageType.RESPONSE, response);
    }
}
