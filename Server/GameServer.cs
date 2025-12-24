using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Common;
using Common.DTO;

namespace Server;

public class GameServer
{
    private readonly TcpListener _listener;
    private readonly GameState _gameState;
    private readonly GameLogic _gameLogic;
    private readonly Dictionary<int, TcpClient> _clients;
    private readonly Random _random;
    private int _nextPlayerId = 0;
    
    public GameServer(int port = 5000)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _gameState = new GameState();
        _gameLogic = new GameLogic(_gameState);
        _clients = new Dictionary<int, TcpClient>();
        _random = new Random();
    }
    
    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine($"Сервер запущен на порту {((IPEndPoint)_listener.LocalEndpoint).Port}");
        
        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Новое подключение");
            _ = Task.Run(() => HandleClient(client));
        }
    }
    
    private async Task HandleClient(TcpClient client)
    {
        int playerId = -1;
        
        try
        {
            var stream = client.GetStream();
            var buffer = new byte[8192];
            
            while (client.Connected)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;
                
                var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var message = JsonSerializer.Deserialize<NetworkMessage>(json);
                
                if (message == null) continue;
                
                var (response, newPlayerId) = await HandleMessage(message, playerId, client);
                if (newPlayerId >= 0)
                    playerId = newPlayerId;
                
                if (response != null)
                {
                    await SendMessage(stream, response);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
        }
        finally
        {
            if (playerId >= 0 && _clients.ContainsKey(playerId))
            {
                _clients.Remove(playerId);
            }
            client.Close();
        }
    }
    
    private async Task<(NetworkMessage?, int)> HandleMessage(NetworkMessage message, int currentPlayerId, TcpClient client)
    {
        switch (message.Type)
        {
            case MessageType.JOIN:
                return await HandleJoin(message, client);
            
            case MessageType.ARCHETYPE:
                return (HandleArchetypeSelection(message, currentPlayerId), currentPlayerId);
            
            case MessageType.MAKE_SOLDIERS:
                return (HandleMakeSoldiers(message, currentPlayerId), currentPlayerId);
            
            case MessageType.ATTACK:
                return (await HandleAttack(message, currentPlayerId), currentPlayerId);
            
            case MessageType.BUILD:
                return (HandleBuild(message, currentPlayerId), currentPlayerId);
            
            case MessageType.UPGRADE:
                return (HandleUpgrade(message, currentPlayerId), currentPlayerId);
            
            case MessageType.END_TURN:
                return (await HandleEndTurn(currentPlayerId), currentPlayerId);
            
            default:
                return (null, currentPlayerId);
        }
    }
    
    private async Task<(NetworkMessage, int)> HandleJoin(NetworkMessage message, TcpClient client)
    {
        var joinDto = JsonSerializer.Deserialize<JoinDto>(message.Payload);
        if (joinDto == null)
        {
            return (CreateResponse(false, ErrorCode.InvalidAction, "Неверные данные"), -1);
        }
        
        if (_gameState.IsStarted)
        {
            return (CreateResponse(false, ErrorCode.InvalidAction, "Игра уже началась"), -1);
        }
        
        if (_gameState.Players.Count >= GameConfig.MaxPlayers)
        {
            return (CreateResponse(false, ErrorCode.InvalidAction, "Игра заполнена"), -1);
        }
        
        var player = new Player(_nextPlayerId++, joinDto.Nickname, joinDto.Email);
        _gameState.Players.Add(player);
        _clients[player.Id] = client;
        
        Console.WriteLine($"Игрок {player.Nickname} присоединился (ID: {player.Id})");
        
        // Если достаточно игроков, начинаем игру
        if (_gameState.Players.Count >= GameConfig.MinPlayers)
        {
            await StartGame();
        }
        
        return (new NetworkMessage
        {
            Type = MessageType.RESPONSE,
            Payload = JsonSerializer.Serialize(new ResponseDto { Success = true })
        }, player.Id);
    }
    
    private async Task StartGame()
    {
        _gameState.IsStarted = true;
        _gameState.Phase = GamePhase.ArchetypeSelection;
        
        Console.WriteLine($"Игра начинается с {_gameState.Players.Count} игроками");
        
        // Отправляем всем игрокам случайные архетипы
        foreach (var player in _gameState.Players)
        {
            var randomArchetype = (ArchetypeType)_random.Next(1, 8); // 1-7, исключая Neutral
            
            var startGameDto = new StartGameDto
            {
                PlayerId = player.Id,
                Players = _gameState.Players.Count,
                Cycles = GameConfig.TotalCycles,
                ArchetypeType = randomArchetype
            };
            
            await BroadcastToPlayer(player.Id, MessageType.START_GAME, startGameDto);
        }
    }
    
    private NetworkMessage HandleArchetypeSelection(NetworkMessage message, int playerId)
    {
        if (playerId < 0)
            return CreateResponse(false, ErrorCode.InvalidAction, "Не авторизован");
        
        var archetypeDto = JsonSerializer.Deserialize<ArchetypeDto>(message.Payload);
        if (archetypeDto == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Неверные данные");
        
        var player = _gameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Игрок не найден");
        
        player.Archetype = archetypeDto.SelectedArchetype;
        
        Console.WriteLine($"Игрок {player.Nickname} выбрал архетип: {player.Archetype}");
        
        // Проверяем, все ли выбрали архетипы
        if (_gameState.Players.All(p => p.Archetype != ArchetypeType.Neutral || p.Archetype == ArchetypeType.Neutral))
        {
            _ = Task.Run(StartFirstTurn);
        }
        
        return CreateResponse(true);
    }
    
    private async Task StartFirstTurn()
    {
        _gameState.Phase = GamePhase.Playing;
        _gameState.CurrentCycle = 1;
        _gameState.ShuffleTurnOrder();
        
        await StartNextPlayerTurn();
    }
    
    private async Task StartNextPlayerTurn()
    {
        var currentPlayer = _gameState.GetCurrentPlayer();
        if (currentPlayer == null) return;
        
        currentPlayer.CurrentTurn++;
        
        // Фаза 1: Производство
        var productionResult = _gameLogic.ExecuteProduction(currentPlayer);
        
        // Фаза 2: Переработка
        var processingResult = _gameLogic.ExecuteProcessing(currentPlayer);
        
        // Объединяем результаты
        var totalProduced = new Dictionary<string, int>();
        foreach (var (res, amt) in productionResult.Produced)
        {
            totalProduced[res] = amt;
        }
        foreach (var (res, amt) in processingResult.Produced)
        {
            if (!totalProduced.ContainsKey(res))
                totalProduced[res] = 0;
            totalProduced[res] += amt;
        }
        
        // Отправляем игроку начало хода
        var startTurnDto = new StartTurnDto
        {
            PlayerId = currentPlayer.Id,
            Cycle = _gameState.CurrentCycle,
            Turn = currentPlayer.CurrentTurn
        };
        
        await BroadcastToPlayer(currentPlayer.Id, MessageType.START_TURN, startTurnDto);
        
        // Отправляем результаты производства
        var finalProductionResult = new ProductionResultDto { Produced = totalProduced };
        await BroadcastToPlayer(currentPlayer.Id, MessageType.PRODUCTION_RESULT, finalProductionResult);
        
        // Отправляем текущее состояние
        await SendStateToPlayer(currentPlayer);
    }
    
    private NetworkMessage HandleMakeSoldiers(NetworkMessage message, int playerId)
    {
        if (playerId < 0)
            return CreateResponse(false, ErrorCode.InvalidAction, "Не авторизован");
        
        var player = _gameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Игрок не найден");
        
        var dto = JsonSerializer.Deserialize<MakeSoldiersRequestDto>(message.Payload);
        if (dto == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Неверные данные");
        
        var result = _gameLogic.MakeSoldiers(player, dto.Count);
        return new NetworkMessage
        {
            Type = MessageType.RESPONSE,
            Payload = JsonSerializer.Serialize(result)
        };
    }
    
    private async Task<NetworkMessage> HandleAttack(NetworkMessage message, int playerId)
    {
        if (playerId < 0)
            return CreateResponse(false, ErrorCode.InvalidAction, "Не авторизован");
        
        var attacker = _gameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (attacker == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Игрок не найден");
        
        var dto = JsonSerializer.Deserialize<AttackRequestDto>(message.Payload);
        if (dto == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Неверные данные");
        
        var target = _gameState.Players.FirstOrDefault(p => p.Id == dto.ToPlayerId);
        if (target == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Цель не найдена");
        
        var result = _gameLogic.ExecuteAttack(attacker, target, dto.Soldiers);
        
        // Уведомляем цель об атаке
        if (result.Success)
        {
            await BroadcastToPlayer(target.Id, MessageType.ATTACK_TARGET, result);
        }
        
        return new NetworkMessage
        {
            Type = MessageType.RESPONSE,
            Payload = JsonSerializer.Serialize(new ResponseDto { Success = result.Success, Message = result.Message })
        };
    }
    
    private NetworkMessage HandleBuild(NetworkMessage message, int playerId)
    {
        if (playerId < 0)
            return CreateResponse(false, ErrorCode.InvalidAction, "Не авторизован");
        
        var player = _gameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Игрок не найден");
        
        var dto = JsonSerializer.Deserialize<BuildRequestDto>(message.Payload);
        if (dto == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Неверные данные");
        
        var result = _gameLogic.BuildBuilding(player, dto.PlaceId, dto.Type);
        return new NetworkMessage
        {
            Type = MessageType.RESPONSE,
            Payload = JsonSerializer.Serialize(result)
        };
    }
    
    private NetworkMessage HandleUpgrade(NetworkMessage message, int playerId)
    {
        if (playerId < 0)
            return CreateResponse(false, ErrorCode.InvalidAction, "Не авторизован");
        
        var player = _gameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Игрок не найден");
        
        var dto = JsonSerializer.Deserialize<UpgradeRequestDto>(message.Payload);
        if (dto == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Неверные данные");
        
        var result = _gameLogic.UpgradeBuilding(player, dto.PlaceId);
        return new NetworkMessage
        {
            Type = MessageType.RESPONSE,
            Payload = JsonSerializer.Serialize(result)
        };
    }
    
    private async Task<NetworkMessage> HandleEndTurn(int playerId)
    {
        if (playerId < 0)
            return CreateResponse(false, ErrorCode.InvalidAction, "Не авторизован");
        
        var player = _gameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return CreateResponse(false, ErrorCode.InvalidAction, "Игрок не найден");
        
        // Уведомляем всех о завершении хода
        await BroadcastToAll(MessageType.TURN_ENDED, new TurnEndedDto { PlayerId = player.Id });
        
        // Переходим к следующему ходу
        _gameState.NextTurn();
        
        if (_gameState.IsFinished)
        {
            await EndGame();
        }
        else
        {
            await StartNextPlayerTurn();
        }
        
        return CreateResponse(true);
    }
    
    private async Task EndGame()
    {
        var results = _gameState.Players
            .Select(p => new { Player = p, Points = p.CalculatePoints() })
            .OrderByDescending(x => x.Points)
            .ToList();
        
        var gameEndDto = new GameEndDto
        {
            Results = results.Select(r => new PlayerResult
            {
                PlayerId = r.Player.Id,
                Nickname = r.Player.Nickname,
                Points = r.Points
            }).ToList()
        };
        
        await BroadcastToAll(MessageType.GAME_END, gameEndDto);
        
        Console.WriteLine("Игра завершена!");
        foreach (var result in results)
        {
            Console.WriteLine($"{result.Player.Nickname}: {result.Points} очков");
        }
    }
    
    private async Task SendStateToPlayer(Player player)
    {
        var stateDto = new StateDto
        {
            Resources = player.Resources,
            Soldiers = player.Soldiers,
            Defense = player.GetDefense(),
            Buildings = player.Field
                .Select((b, i) => b != null ? new BuildingStateDto
                {
                    PlaceId = i,
                    Type = b.Type,
                    Level = b.Level
                } : null)
                .Where(b => b != null)
                .Cast<BuildingStateDto>()
                .ToList()
        };
        
        await BroadcastToPlayer(player.Id, MessageType.STATE, stateDto);
    }
    
    private async Task BroadcastToPlayer(int playerId, MessageType type, object dto)
    {
        if (!_clients.TryGetValue(playerId, out var client))
            return;
        
        var message = new NetworkMessage
        {
            Type = type,
            Payload = JsonSerializer.Serialize(dto)
        };
        
        await SendMessage(client.GetStream(), message);
    }
    
    private async Task BroadcastToAll(MessageType type, object dto)
    {
        var message = new NetworkMessage
        {
            Type = type,
            Payload = JsonSerializer.Serialize(dto)
        };
        
        foreach (var client in _clients.Values)
        {
            await SendMessage(client.GetStream(), message);
        }
    }
    
    private async Task SendMessage(NetworkStream stream, NetworkMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(bytes, 0, bytes.Length);
    }
    
    private NetworkMessage CreateResponse(bool success, ErrorCode? errorCode = null, string? message = null)
    {
        return new NetworkMessage
        {
            Type = MessageType.RESPONSE,
            Payload = JsonSerializer.Serialize(new ResponseDto
            {
                Success = success,
                ErrorCode = errorCode,
                Message = message
            })
        };
    }
}
