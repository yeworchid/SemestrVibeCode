using Common;
using Common.DTO;
using Common.Services;
using System.Net;
using System.Net.Sockets;

namespace Server;

public class GameServer
{
    private TcpListener listener = null!;
    private List<Player> players = new List<Player>();
    private int nextId = 1;
    private bool gameStarted = false;
    private int cycle = 0;
    private int turnIdx = 0;
    private List<int> turnOrder = new List<int>();
    private int totalCycles = 15;
    private int globalTurn = 0;
    private Random rnd = new Random();

    public void ForceStart()
    {
        if (!gameStarted && players.Count >= 2)
        {
            StartGame();
        }
    }

    public void Start(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Сервер запущен на порту " + port);

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("Клиент подключился");
            Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[4096];
            int offset = 0;

            while (true)
            {
                int read = stream.Read(buffer, offset, buffer.Length - offset);
                if (read == 0) break;
                offset += read;

                while (ByteConverter.TryReadMessage(buffer, 0, offset, out string msgStr, out int bytesUsed))
                {
                    var msg = MessageParser.Parse(msgStr);
                    ProcessMessage(client, stream, msg);

                    Buffer.BlockCopy(buffer, bytesUsed, buffer, 0, offset - bytesUsed);
                    offset -= bytesUsed;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }

    private void ProcessMessage(TcpClient client, NetworkStream stream, NetworkMessage msg)
    {
        if (msg.Type == MessageType.JOIN)
        {
            var dto = MessageDeserializer.Deserialize<JoinDto>(msg);
            var p = new Player
            {
                Id = nextId++,
                Nickname = dto?.Nickname ?? "Player",
                Email = dto?.Email ?? "",
                Client = client,
                Stream = stream
            };
            players.Add(p);
            Console.WriteLine("Игрок " + p.Nickname + " присоединился (id=" + p.Id + ")");

            SendResponse(p, true, "Подключено");

            if (players.Count == 4 && !gameStarted)
            {
                StartGame();
            }
            else if (players.Count >= 2 && !gameStarted)
            {
                Console.WriteLine("Ожидание игроков... (" + players.Count + "/4)");
                Console.WriteLine("Для начала игры с " + players.Count + " игроками введите 'start'");
            }
        }
        else
        {
            var player = players.Find(x => x.Client == client);
            if (player != null)
            {
                HandlePlayerMsg(player, msg);
            }
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        Console.WriteLine("Игра начинается!");

        var playerInfos = new List<PlayerInfoDto>();
        foreach (var pl in players)
        {
            playerInfos.Add(new PlayerInfoDto
            {
                Id = pl.Id,
                Nickname = pl.Nickname,
                Email = pl.Email
            });
        }

        foreach (var p in players)
        {
            var dto = new StartGameDto
            {
                PlayerId = p.Id,
                PlayerCount = players.Count,
                Players = playerInfos
            };
            SendMsg(p, MessageType.START_GAME, dto);
        }
    }

    private void HandlePlayerMsg(Player p, NetworkMessage msg)
    {
        if (msg.Type == MessageType.ARCHETYPE)
        {
            var dto = MessageDeserializer.Deserialize<ArchetypeDto>(msg);
            if (dto != null)
            {
                p.Archetype = dto.ArchetypeType;
                p.InitResources();
                Console.WriteLine("Игрок " + p.Nickname + " выбрал архетип " + p.Archetype);

                bool allReady = true;
                foreach (var pl in players)
                {
                    if (pl.Resources.Count == 0)
                    {
                        allReady = false;
                        break;
                    }
                }

                if (allReady)
                {
                    StartFirstCycle();
                }
            }
        }
        else if (msg.Type == MessageType.BUILD)
        {
            var dto = MessageDeserializer.Deserialize<BuildRequestDto>(msg);
            if (dto != null) DoBuild(p, dto);
        }
        else if (msg.Type == MessageType.UPGRADE)
        {
            var dto = MessageDeserializer.Deserialize<UpgradeRequestDto>(msg);
            if (dto != null) DoUpgrade(p, dto);
        }
        else if (msg.Type == MessageType.MAKE_SOLDIERS)
        {
            var dto = MessageDeserializer.Deserialize<MakeSoldiersRequestDto>(msg);
            if (dto != null) DoMakeSoldiers(p, dto);
        }
        else if (msg.Type == MessageType.ATTACK)
        {
            var dto = MessageDeserializer.Deserialize<AttackRequestDto>(msg);
            if (dto != null) DoAttack(p, dto);
        }
        else if (msg.Type == MessageType.END_TURN)
        {
            DoEndTurn(p);
        }
    }

    private void StartFirstCycle()
    {
        cycle = 1;
        globalTurn = 1;
        ShuffleTurnOrder();
        turnIdx = 0;
        StartPlayerTurn();
    }

    private void ShuffleTurnOrder()
    {
        turnOrder.Clear();
        foreach (var p in players)
            turnOrder.Add(p.Id);

        for (int i = turnOrder.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            int tmp = turnOrder[i];
            turnOrder[i] = turnOrder[j];
            turnOrder[j] = tmp;
        }
    }

    private void StartPlayerTurn()
    {
        int pid = turnOrder[turnIdx];
        var p = players.Find(x => x.Id == pid);
        if (p == null) return;

        DoProduction(p);
        DoProcessing(p);

        var turnDto = new StartTurnDto
        {
            PlayerId = pid,
            Cycle = cycle,
            Turn = globalTurn
        };

        foreach (var pl in players)
        {
            SendMsg(pl, MessageType.START_TURN, turnDto);
        }

        SendState(p);
    }

    private void DoProduction(Player p)
    {
        var produced = new Dictionary<string, int>();

        var sorted = p.Buildings.OrderBy(b => b.PlaceId).ToList();
        foreach (var b in sorted)
        {
            if (GameLogic.IsProducer(b.Type))
            {
                string res = GameLogic.GetProducerOutput(b.Type);
                int amt = GameLogic.GetProduction(b.Type, b.Level);
                p.AddResource(res, amt);

                if (!produced.ContainsKey(res))
                    produced[res] = 0;
                produced[res] += amt;
            }
        }

        var dto = new ProductionResultDto { ProducedResources = produced };
        SendMsg(p, MessageType.PRODUCTION_RESULT, dto);
    }

    private void DoProcessing(Player p)
    {
        var sorted = p.Buildings.OrderBy(b => b.PlaceId).ToList();
        foreach (var b in sorted)
        {
            if (GameLogic.IsProcessor(b.Type))
            {
                var input = GameLogic.GetProcessorInput(b.Type);
                string output = GameLogic.GetProcessorOutput(b.Type);
                int maxTimes = GameLogic.GetProduction(b.Type, b.Level);

                for (int i = 0; i < maxTimes; i++)
                {
                    bool canProcess = true;
                    foreach (var inp in input)
                    {
                        if (!p.HasResource(inp.Key, inp.Value))
                        {
                            canProcess = false;
                            break;
                        }
                    }

                    if (canProcess)
                    {
                        foreach (var inp in input)
                            p.RemoveResource(inp.Key, inp.Value);
                        p.AddResource(output, 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void DoBuild(Player p, BuildRequestDto dto)
    {
        foreach (var b in p.Buildings)
        {
            if (b.PlaceId == dto.PlaceId)
            {
                SendResponse(p, false, "Место занято");
                return;
            }
        }

        var cost = GameLogic.GetBuildCost(dto.Type);
        foreach (var c in cost)
        {
            if (!p.HasResource(c.Key, c.Value))
            {
                SendResponse(p, false, "Не хватает ресурсов");
                return;
            }
        }

        foreach (var c in cost)
            p.RemoveResource(c.Key, c.Value);

        var building = new Building
        {
            PlaceId = dto.PlaceId,
            Type = dto.Type,
            Level = 1,
            TurnBuilt = globalTurn
        };
        p.Buildings.Add(building);

        if (p.Archetype == ArchetypeType.Engineer)
        {
            var costList = cost.Keys.ToList();
            int refund = rnd.Next(1, 3);
            for (int i = 0; i < refund && costList.Count > 0; i++)
            {
                string res = costList[rnd.Next(costList.Count)];
                p.AddResource(res, 1);
            }
        }

        SendResponse(p, true, "Построено");
        SendState(p);
    }

    private void DoUpgrade(Player p, UpgradeRequestDto dto)
    {
        Building? b = null;
        foreach (var bld in p.Buildings)
        {
            if (bld.PlaceId == dto.PlaceId)
            {
                b = bld;
                break;
            }
        }

        if (b == null)
        {
            SendResponse(p, false, "Здание не найдено");
            return;
        }

        if (b.TurnBuilt == globalTurn)
        {
            SendResponse(p, false, "Нельзя улучшить в тот же ход");
            return;
        }

        if (b.Level >= 3)
        {
            SendResponse(p, false, "Максимальный уровень");
            return;
        }

        var cost = GameLogic.GetUpgradeCost(b.Type, b.Level + 1);
        foreach (var c in cost)
        {
            if (!p.HasResource(c.Key, c.Value))
            {
                SendResponse(p, false, "Не хватает ресурсов");
                return;
            }
        }

        foreach (var c in cost)
            p.RemoveResource(c.Key, c.Value);

        b.Level++;
        b.TurnBuilt = globalTurn;

        SendResponse(p, true, "Улучшено");
        SendState(p);
    }

    private void DoMakeSoldiers(Player p, MakeSoldiersRequestDto dto)
    {
        Building? barracks = null;
        foreach (var b in p.Buildings)
        {
            if (b.PlaceId == dto.BarracksId && b.Type == BuildingType.Barracks)
            {
                barracks = b;
                break;
            }
        }

        if (barracks == null)
        {
            SendResponse(p, false, "Казармы не найдены");
            return;
        }

        int maxSoldiers = GameLogic.GetProduction(BuildingType.Barracks, barracks.Level);
        if (dto.Count > maxSoldiers)
        {
            SendResponse(p, false, "Слишком много солдат");
            return;
        }

        var soldierCost = GameLogic.GetSoldierCost(p.Archetype);
        foreach (var c in soldierCost)
        {
            if (!p.HasResource(c.Key, c.Value * dto.Count))
            {
                SendResponse(p, false, "Не хватает ресурсов");
                return;
            }
        }

        foreach (var c in soldierCost)
            p.RemoveResource(c.Key, c.Value * dto.Count);

        p.Soldiers += dto.Count;

        SendResponse(p, true, "Солдаты созданы");
        SendState(p);
    }

    private void DoAttack(Player p, AttackRequestDto dto)
    {
        if (cycle <= 5)
        {
            SendResponse(p, false, "Атака запрещена первые 5 циклов");
            return;
        }

        Player? target = null;
        foreach (var pl in players)
        {
            if (pl.Id == dto.ToPlayerId)
            {
                target = pl;
                break;
            }
        }

        if (target == null || target.Id == p.Id)
        {
            SendResponse(p, false, "Неверная цель");
            return;
        }

        if (p.Soldiers < dto.Soldiers)
        {
            SendResponse(p, false, "Недостаточно солдат");
            return;
        }

        p.Soldiers -= dto.Soldiers;

        int defense = target.GetDefense();
        if (p.Archetype == ArchetypeType.Warrior)
            defense = (int)(defense * 0.8);
        else if (p.Archetype == ArchetypeType.Recruit)
            defense = (int)(defense * 1.2);

        if (defense > 100) defense = 100;

        int lost = (int)Math.Ceiling(dto.Soldiers * (defense / 100.0));
        int survived = dto.Soldiers - lost;

        var stolen = new Dictionary<string, int>();
        int stealsPerSoldier = 1;
        if (p.Archetype == ArchetypeType.Glutton)
            stealsPerSoldier = 2;

        for (int i = 0; i < survived * stealsPerSoldier; i++)
        {
            var available = new List<string>();
            foreach (var r in target.Resources)
            {
                if (r.Value > 0)
                    available.Add(r.Key);
            }

            if (available.Count > 0)
            {
                string res = available[rnd.Next(available.Count)];
                target.RemoveResource(res, 1);
                
                if (!stolen.ContainsKey(res))
                    stolen[res] = 0;
                stolen[res]++;
            }
        }

        foreach (var s in stolen)
            p.AddResource(s.Key, s.Value);

        p.Soldiers += survived;

        var attackDto = new AttackTargetDto
        {
            ToPlayerId = dto.ToPlayerId,
            Sent = dto.Soldiers,
            Lost = lost,
            StolenResources = stolen
        };
        SendMsg(p, MessageType.ATTACK_TARGET, attackDto);
        SendState(p);
    }

    private void DoEndTurn(Player p)
    {
        turnIdx++;
        globalTurn++;

        if (turnIdx >= turnOrder.Count)
        {
            cycle++;
            turnIdx = 0;
            ShuffleTurnOrder();

            if (cycle > totalCycles)
            {
                EndGame();
                return;
            }
        }

        var dto = new TurnEndedDto
        {
            PlayerId = p.Id,
            NextPlayerId = turnOrder[turnIdx]
        };

        foreach (var pl in players)
        {
            SendMsg(pl, MessageType.TURN_ENDED, dto);
        }

        StartPlayerTurn();
    }

    private void EndGame()
    {
        int maxPts = 0;
        Player? winner = null;

        var allScores = new List<PlayerScoreDto>();

        foreach (var p in players)
        {
            int pts = p.CalcPoints();
            Console.WriteLine("Игрок " + p.Nickname + ": " + pts + " очков");
            
            allScores.Add(new PlayerScoreDto
            {
                PlayerId = p.Id,
                Nickname = p.Nickname,
                Points = pts
            });

            if (pts > maxPts)
            {
                maxPts = pts;
                winner = p;
            }
        }

        var dto = new GameEndDto
        {
            WinnerPlayerId = winner?.Id ?? 0,
            Points = maxPts,
            AllScores = allScores
        };

        foreach (var p in players)
        {
            SendMsg(p, MessageType.GAME_END, dto);
        }

        Console.WriteLine("Игра окончена! Победитель: " + (winner?.Nickname ?? "никто"));
    }

    private void SendState(Player p)
    {
        var buildings = new List<BuildingStateDto>();
        foreach (var b in p.Buildings)
        {
            buildings.Add(new BuildingStateDto
            {
                PlaceId = b.PlaceId,
                Type = b.Type,
                Level = b.Level
            });
        }

        var dto = new StateDto
        {
            Resources = new Dictionary<string, int>(p.Resources),
            Soldiers = p.Soldiers,
            Defense = p.GetDefense(),
            Buildings = buildings
        };
        SendMsg(p, MessageType.STATE, dto);
    }

    private void SendResponse(Player p, bool success, string message)
    {
        var dto = new ResponseDto
        {
            Success = success,
            Message = message
        };
        SendMsg(p, MessageType.RESPONSE, dto);
    }

    private void SendMsg(Player p, MessageType type, object payload)
    {
        try
        {
            var msg = MessageSerializer.Serialize(type, payload);
            string str = MessageParser.Serialize(msg);
            byte[] data = ByteConverter.StringToBytes(str);
            p.Stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка отправки: " + ex.Message);
        }
    }
}
