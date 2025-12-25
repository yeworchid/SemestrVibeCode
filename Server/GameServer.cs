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
        Console.WriteLine("╔════════════════════════════════════════════╗");
        Console.WriteLine("║   ⚔️ БОЙЦЫ ХЛОПКОВЫХ ПЛАНТАЦИЙ 2 ⚔️        ║");
        Console.WriteLine("║          Сервер запущен на порту " + port + "       ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("🎮 Боец подключился к плантации!");
            Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(TcpClient client)
    {
        Player? disconnectedPlayer = null;
        
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
            Console.WriteLine("Ошибка соединения: " + ex.Message);
        }
        finally
        {
            // Игрок отключился
            disconnectedPlayer = players.Find(x => x.Client == client);
            if (disconnectedPlayer != null)
            {
                HandlePlayerDisconnect(disconnectedPlayer);
            }
        }
    }

    private void HandlePlayerDisconnect(Player p)
    {
        Console.WriteLine($"❌ Боец {p.Nickname} (#{p.Id}) покинул плантацию!");

        bool wasCurrentTurn = false;
        
        // Проверяем, был ли это ход отключившегося игрока
        if (gameStarted && turnOrder.Count > 0 && turnIdx < turnOrder.Count)
        {
            wasCurrentTurn = turnOrder[turnIdx] == p.Id;
        }

        // Удаляем игрока из списка
        players.Remove(p);
        turnOrder.Remove(p.Id);

        // Если игра ещё не началась или осталось меньше 2 игроков
        if (!gameStarted || players.Count < 2)
        {
            if (players.Count < 2 && gameStarted)
            {
                Console.WriteLine("⚠️ Недостаточно игроков для продолжения!");
                if (players.Count == 1)
                {
                    // Единственный оставшийся игрок побеждает
                    EndGameWithWinner(players[0]);
                }
                return;
            }
            
            // Уведомляем оставшихся игроков
            NotifyPlayerLeft(p);
            return;
        }

        // Уведомляем оставшихся игроков
        NotifyPlayerLeft(p);

        // Если был ход отключившегося игрока - переходим к следующему
        if (wasCurrentTurn)
        {
            Console.WriteLine($"[Сервер] Ход {p.Nickname} прерван, переход к следующему игроку");
            
            // Корректируем индекс если нужно
            if (turnIdx >= turnOrder.Count)
            {
                // Начинаем новый цикл
                cycle++;
                turnIdx = 0;
                ShuffleTurnOrder();
                Console.WriteLine($"=== Начало цикла {cycle} ===");

                if (cycle > totalCycles)
                {
                    EndGame();
                    return;
                }
            }

            // Уведомляем о смене хода и начинаем ход следующего игрока
            if (turnOrder.Count > 0)
            {
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
        }
    }

    private void NotifyPlayerLeft(Player leftPlayer)
    {
        var remainingPlayers = new List<PlayerInfoDto>();
        foreach (var pl in players)
        {
            remainingPlayers.Add(new PlayerInfoDto
            {
                Id = pl.Id,
                Nickname = pl.Nickname,
                Email = pl.Email
            });
        }

        var dto = new PlayerLeftDto
        {
            PlayerId = leftPlayer.Id,
            Nickname = leftPlayer.Nickname,
            RemainingPlayers = remainingPlayers
        };

        foreach (var pl in players)
        {
            SendMsg(pl, MessageType.PLAYER_LEFT, dto);
        }
    }

    private void EndGameWithWinner(Player winner)
    {
        int pts = winner.CalcPoints();
        Console.WriteLine($"🏆 {winner.Nickname} побеждает по умолчанию с {pts} очками!");

        var allScores = new List<PlayerScoreDto>
        {
            new PlayerScoreDto
            {
                PlayerId = winner.Id,
                Nickname = winner.Nickname,
                Points = pts
            }
        };

        var dto = new GameEndDto
        {
            WinnerPlayerId = winner.Id,
            Points = pts,
            AllScores = allScores
        };

        SendMsg(winner, MessageType.GAME_END, dto);
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
        Console.WriteLine("🌾 БИТВА НА ПЛАНТАЦИИ НАЧИНАЕТСЯ! 🌾");

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
                    if (pl.ResourceStorage.Count == 0)
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

        // Сброс счётчиков за ход
        p.SoldiersCreatedThisTurn = 0;
        p.AttackedPlayersThisTurn.Clear();

        Console.WriteLine($"=== Ход игрока {p.Nickname} (цикл {cycle}, ход {globalTurn}) ===");

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
        var produced = new Dictionary<Resources, int>();

        var sorted = p.Buildings.OrderBy(b => b.PlaceId).ToList();
        foreach (var b in sorted)
        {
            if (GameLogic.IsProducer(b.Type))
            {
                Resources res = GameLogic.GetProducerOutput(b.Type);
                int amt = GameLogic.GetProduction(b.Type, b.Level);
                p.AddResource(res, amt);

                if (!produced.ContainsKey(res))
                    produced[res] = 0;
                produced[res] += amt;
            }
        }

        var dto = new ProductionResultDto { ProducedResources = produced.ToDictionary(x => x.Key.ToString(), x => x.Value) };
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
                Resources output = GameLogic.GetProcessorOutput(b.Type);
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
        Console.WriteLine($"[{p.Nickname}] Запрос на постройку {dto.Type} на месте {dto.PlaceId}");

        foreach (var b in p.Buildings)
        {
            if (b.PlaceId == dto.PlaceId)
            {
                Console.WriteLine($"[{p.Nickname}] Отказ: место {dto.PlaceId} занято");
                SendResponse(p, false, "Место занято");
                return;
            }
        }

        var cost = GameLogic.GetBuildCost(dto.Type);
        foreach (var c in cost)
        {
            if (!p.HasResource(c.Key, c.Value))
            {
                Console.WriteLine($"[{p.Nickname}] Отказ: не хватает {c.Key}");
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
                Resources res = costList[rnd.Next(costList.Count)];
                p.AddResource(res, 1);
                Console.WriteLine($"[{p.Nickname}] Инженер: возврат 1 {res}");
            }
        }

        Console.WriteLine($"[{p.Nickname}] Построено {dto.Type} на месте {dto.PlaceId}");
        SendResponse(p, true, "Построено");
        SendState(p);
    }

    private void DoUpgrade(Player p, UpgradeRequestDto dto)
    {
        Console.WriteLine($"[{p.Nickname}] Запрос на улучшение здания на месте {dto.PlaceId}");

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
            Console.WriteLine($"[{p.Nickname}] Отказ: здание не найдено");
            SendResponse(p, false, "Здание не найдено");
            return;
        }

        if (b.TurnBuilt == globalTurn)
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: нельзя улучшить в тот же ход");
            SendResponse(p, false, "Нельзя улучшить в тот же ход");
            return;
        }

        if (b.Level >= 3)
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: максимальный уровень");
            SendResponse(p, false, "Максимальный уровень");
            return;
        }

        var cost = GameLogic.GetUpgradeCost(b.Type, b.Level + 1);
        foreach (var c in cost)
        {
            if (!p.HasResource(c.Key, c.Value))
            {
                Console.WriteLine($"[{p.Nickname}] Отказ: не хватает {c.Key}");
                SendResponse(p, false, "Не хватает ресурсов");
                return;
            }
        }

        foreach (var c in cost)
            p.RemoveResource(c.Key, c.Value);

        b.Level++;
        b.TurnBuilt = globalTurn;

        Console.WriteLine($"[{p.Nickname}] Улучшено {b.Type} до уровня {b.Level}");
        SendResponse(p, true, "Улучшено");
        SendState(p);
    }

    private void DoMakeSoldiers(Player p, MakeSoldiersRequestDto dto)
    {
        Console.WriteLine($"[{p.Nickname}] Запрос на создание {dto.Count} солдат");

        int maxSoldiersPerTurn = p.GetMaxSoldiersPerTurn();
        int canCreate = maxSoldiersPerTurn - p.SoldiersCreatedThisTurn;

        if (canCreate <= 0)
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: лимит солдат за ход исчерпан ({p.SoldiersCreatedThisTurn}/{maxSoldiersPerTurn})");
            SendResponse(p, false, "Лимит солдат за ход исчерпан");
            return;
        }

        if (dto.Count > canCreate)
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: запрошено {dto.Count}, доступно {canCreate}");
            SendResponse(p, false, $"Можно создать ещё {canCreate} солдат в этот ход");
            return;
        }

        var soldierCost = GameLogic.GetSoldierCost(p.Archetype);
        foreach (var c in soldierCost)
        {
            if (!p.HasResource(c.Key, c.Value * dto.Count))
            {
                Console.WriteLine($"[{p.Nickname}] Отказ: не хватает ресурсов");
                SendResponse(p, false, "Не хватает ресурсов");
                return;
            }
        }

        foreach (var c in soldierCost)
            p.RemoveResource(c.Key, c.Value * dto.Count);

        p.Soldiers += dto.Count;
        p.SoldiersCreatedThisTurn += dto.Count;

        Console.WriteLine($"[{p.Nickname}] Создано {dto.Count} солдат (всего: {p.Soldiers}, за ход: {p.SoldiersCreatedThisTurn}/{maxSoldiersPerTurn})");
        SendResponse(p, true, "Солдаты созданы");
        SendState(p);
    }

    private void DoAttack(Player p, AttackRequestDto dto)
    {
        Console.WriteLine($"[{p.Nickname}] Запрос на атаку игрока {dto.ToPlayerId} с {dto.Soldiers} солдатами");

        if (cycle <= 5)
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: атака запрещена первые 5 циклов");
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
            Console.WriteLine($"[{p.Nickname}] Отказ: неверная цель");
            SendResponse(p, false, "Неверная цель");
            return;
        }

        // Проверка: можно атаковать каждого игрока только 1 раз за ход
        if (p.AttackedPlayersThisTurn.Contains(target.Id))
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: уже атаковал {target.Nickname} в этот ход");
            SendResponse(p, false, "Вы уже атаковали этого игрока в этот ход");
            return;
        }

        if (p.Soldiers < dto.Soldiers)
        {
            Console.WriteLine($"[{p.Nickname}] Отказ: недостаточно солдат ({p.Soldiers} < {dto.Soldiers})");
            SendResponse(p, false, "Недостаточно солдат");
            return;
        }

        p.Soldiers -= dto.Soldiers;
        p.AttackedPlayersThisTurn.Add(target.Id);

        int defense = target.GetDefense();
        int originalDefense = defense;
        if (p.Archetype == ArchetypeType.Warrior)
            defense = (int)(defense * 0.8);
        else if (p.Archetype == ArchetypeType.Recruit)
            defense = (int)(defense * 1.2);

        if (defense > 100) defense = 100;

        int lost = (int)Math.Ceiling(dto.Soldiers * (defense / 100.0));
        int survived = dto.Soldiers - lost;

        Console.WriteLine($"[{p.Nickname}] Атака на {target.Nickname}: оборона {originalDefense}% -> {defense}%, потери {lost}, выжило {survived}");

        var stolen = new Dictionary<Resources, int>();
        int stealsPerSoldier = 1;
        if (p.Archetype == ArchetypeType.Glutton)
            stealsPerSoldier = 2;

        for (int i = 0; i < survived * stealsPerSoldier; i++)
        {
            var available = new List<Resources>();
            foreach (var r in target.ResourceStorage)
            {
                if (r.Value > 0)
                    available.Add(r.Key);
            }

            if (available.Count > 0)
            {
                Resources res = available[rnd.Next(available.Count)];
                target.RemoveResource(res, 1);
                
                if (!stolen.ContainsKey(res))
                    stolen[res] = 0;
                stolen[res]++;
            }
        }

        foreach (var s in stolen)
            p.AddResource(s.Key, s.Value);

        p.Soldiers += survived;

        string stolenStr = string.Join(", ", stolen.Select(x => $"{x.Key}:{x.Value}"));
        Console.WriteLine($"[{p.Nickname}] Украдено: {stolenStr}");

        // Конвертируем в string для DTO
        var stolenStrDict = stolen.ToDictionary(x => x.Key.ToString(), x => x.Value);

        // Уведомление атакующему
        var attackDto = new AttackTargetDto
        {
            ToPlayerId = dto.ToPlayerId,
            Sent = dto.Soldiers,
            Lost = lost,
            StolenResources = stolenStrDict
        };
        SendMsg(p, MessageType.ATTACK_TARGET, attackDto);

        // Уведомление атакуемому
        var receivedDto = new AttackReceivedDto
        {
            FromPlayerId = p.Id,
            FromNickname = p.Nickname,
            SoldiersAttacked = dto.Soldiers,
            SoldiersLost = lost,
            LostResources = stolenStrDict
        };
        SendMsg(target, MessageType.ATTACK_RECEIVED, receivedDto);

        // Отправляем RESPONSE чтобы сбросить флаг ожидания
        SendResponse(p, true, "Атака выполнена");
        
        SendState(p);
        SendState(target);
    }

    private void DoEndTurn(Player p)
    {
        Console.WriteLine($"[{p.Nickname}] Завершает ход");

        turnIdx++;
        globalTurn++;

        if (turnIdx >= turnOrder.Count)
        {
            cycle++;
            turnIdx = 0;
            ShuffleTurnOrder();
            Console.WriteLine($"=== Начало цикла {cycle} ===");

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

        Console.WriteLine("🏆 БИТВА ОКОНЧЕНА! Победитель плантации: " + (winner?.Nickname ?? "никто") + " 🏆");
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
            Resources = p.ResourceStorage.ToDictionary(x => x.Key.ToString(), x => x.Value),
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
