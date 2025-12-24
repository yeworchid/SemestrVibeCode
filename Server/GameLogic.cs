using Common;
using Common.DTO;

namespace Server;

public class GameLogic
{
    private readonly GameState _state;
    private readonly Random _random;
    
    public GameLogic(GameState state)
    {
        _state = state;
        _random = new Random();
    }
    
    // Фаза 1: Производство
    public ProductionResultDto ExecuteProduction(Player player)
    {
        var produced = new Dictionary<string, int>();
        
        for (int i = 0; i < player.Field.Length; i++)
        {
            var building = player.Field[i];
            if (building == null) continue;
            
            if (!BuildingConfig.Buildings.TryGetValue(building.Type, out var info))
                continue;
            
            if (info.Category == BuildingCategory.Producer)
            {
                var levelInfo = info.Levels[building.Level];
                if (levelInfo.Output != null)
                {
                    foreach (var (resource, amount) in levelInfo.Output)
                    {
                        if (!produced.ContainsKey(resource))
                            produced[resource] = 0;
                        produced[resource] += amount;
                    }
                }
            }
        }
        
        player.AddResources(produced);
        
        return new ProductionResultDto
        {
            Produced = produced
        };
    }
    
    // Фаза 2: Переработка
    public ProductionResultDto ExecuteProcessing(Player player)
    {
        var produced = new Dictionary<string, int>();
        
        for (int i = 0; i < player.Field.Length; i++)
        {
            var building = player.Field[i];
            if (building == null) continue;
            
            if (!BuildingConfig.Buildings.TryGetValue(building.Type, out var info))
                continue;
            
            if (info.Category == BuildingCategory.Processor || info.Category == BuildingCategory.Precious)
            {
                var levelInfo = info.Levels[building.Level];
                if (levelInfo.Input != null && levelInfo.Output != null)
                {
                    // Проверяем, хватает ли ресурсов
                    if (player.HasResources(levelInfo.Input))
                    {
                        player.ConsumeResources(levelInfo.Input);
                        
                        foreach (var (resource, amount) in levelInfo.Output)
                        {
                            if (!produced.ContainsKey(resource))
                                produced[resource] = 0;
                            produced[resource] += amount;
                        }
                        
                        player.AddResources(levelInfo.Output);
                    }
                }
            }
        }
        
        return new ProductionResultDto
        {
            Produced = produced
        };
    }
    
    // Фаза 3: Производство солдат
    public ResponseDto MakeSoldiers(Player player, int count)
    {
        // Находим казармы и их максимальную производительность
        int maxProduction = 0;
        foreach (var building in player.Field)
        {
            if (building?.Type == BuildingType.Barracks)
            {
                var info = BuildingConfig.Buildings[BuildingType.Barracks];
                var levelInfo = info.Levels[building.Level];
                if (levelInfo.Output != null && levelInfo.Output.ContainsKey("Soldier"))
                {
                    maxProduction += levelInfo.Output["Soldier"];
                }
            }
        }
        
        if (count > maxProduction)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = $"Можно произвести максимум {maxProduction} солдат за ход"
            };
        }
        
        // Рассчитываем стоимость с учетом архетипа
        var cost = GetSoldierCost(player.Archetype, count);
        
        if (!player.HasResources(cost))
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.NotEnoughResources,
                Message = "Недостаточно ресурсов для производства солдат"
            };
        }
        
        player.ConsumeResources(cost);
        player.Soldiers += count;
        
        return new ResponseDto { Success = true };
    }
    
    private Dictionary<string, int> GetSoldierCost(ArchetypeType archetype, int count)
    {
        var baseCost = archetype switch
        {
            ArchetypeType.Warrior => new Dictionary<string, int> { { "Bread", 5 }, { "Weapon", 2 } },
            ArchetypeType.Recruit => new Dictionary<string, int> { { "Bread", 1 }, { "Weapon", 1 } },
            ArchetypeType.Glutton => new Dictionary<string, int> { { "Bread", 6 }, { "Weapon", 1 } },
            _ => new Dictionary<string, int> { { "Bread", 3 }, { "Weapon", 1 } }
        };
        
        return baseCost.ToDictionary(kv => kv.Key, kv => kv.Value * count);
    }
    
    // Фаза 4: Атака
    public AttackTargetDto ExecuteAttack(Player attacker, Player target, int soldierCount)
    {
        if (_state.CurrentCycle < GameConfig.PeaceProtectionCycles)
        {
            return new AttackTargetDto
            {
                Success = false,
                Message = "Атаки запрещены в первые 5 циклов"
            };
        }
        
        if (attacker.Soldiers < soldierCount)
        {
            return new AttackTargetDto
            {
                Success = false,
                Message = "Недостаточно солдат"
            };
        }
        
        // Рассчитываем потери
        int targetDefense = target.GetDefense();
        
        // Модификаторы архетипа атакующего
        if (attacker.Archetype == ArchetypeType.Warrior)
            targetDefense = Math.Max(0, targetDefense - 20);
        else if (attacker.Archetype == ArchetypeType.Recruit)
            targetDefense += 20;
        
        double lossRate = Math.Min(1.0, targetDefense / 100.0);
        int losses = (int)Math.Ceiling(soldierCount * lossRate);
        int survivors = soldierCount - losses;
        
        attacker.Soldiers -= soldierCount;
        
        // Воруем ресурсы
        var stolen = new Dictionary<string, int>();
        int resourcesPerSoldier = attacker.Archetype == ArchetypeType.Glutton ? 2 : 1;
        int totalResourcesToSteal = survivors * resourcesPerSoldier;
        
        var availableResources = target.Resources.Where(kv => kv.Value > 0).ToList();
        
        for (int i = 0; i < totalResourcesToSteal && availableResources.Count > 0; i++)
        {
            var randomIndex = _random.Next(availableResources.Count);
            var (resource, _) = availableResources[randomIndex];
            
            if (target.Resources[resource] > 0)
            {
                target.Resources[resource]--;
                if (target.Resources[resource] == 0)
                {
                    target.Resources.Remove(resource);
                    availableResources.RemoveAt(randomIndex);
                }
                
                if (!stolen.ContainsKey(resource))
                    stolen[resource] = 0;
                stolen[resource]++;
            }
        }
        
        attacker.AddResources(stolen);
        attacker.Soldiers += survivors;
        
        return new AttackTargetDto
        {
            Success = true,
            Losses = losses,
            Survivors = survivors,
            StolenResources = stolen
        };
    }
    
    // Фаза 5: Строительство
    public ResponseDto BuildBuilding(Player player, int placeId, BuildingType buildingType)
    {
        if (placeId < 0 || placeId >= player.Field.Length)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Неверный номер клетки"
            };
        }
        
        if (player.Field[placeId] != null)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Клетка уже занята"
            };
        }
        
        if (!BuildingConfig.Buildings.TryGetValue(buildingType, out var info))
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Неизвестный тип здания"
            };
        }
        
        if (!player.HasResources(info.BuildCost))
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.NotEnoughResources,
                Message = "Недостаточно ресурсов для строительства"
            };
        }
        
        player.ConsumeResources(info.BuildCost);
        
        // Инженер возвращает 1-2 случайных ресурса
        if (player.Archetype == ArchetypeType.Engineer)
        {
            var refundCount = _random.Next(1, 3);
            var costList = info.BuildCost.ToList();
            for (int i = 0; i < refundCount && costList.Count > 0; i++)
            {
                var randomIndex = _random.Next(costList.Count);
                var (resource, _) = costList[randomIndex];
                player.AddResources(new Dictionary<string, int> { { resource, 1 } });
            }
        }
        
        player.Field[placeId] = new Building(buildingType, placeId, player.CurrentTurn);
        
        return new ResponseDto { Success = true };
    }
    
    public ResponseDto UpgradeBuilding(Player player, int placeId)
    {
        if (placeId < 0 || placeId >= player.Field.Length)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Неверный номер клетки"
            };
        }
        
        var building = player.Field[placeId];
        if (building == null)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "На этой клетке нет здания"
            };
        }
        
        if (building.TurnBuilt == player.CurrentTurn)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.UpgradeNotAllowed,
                Message = "Нельзя улучшить здание в тот же ход, когда оно построено"
            };
        }
        
        if (building.Level >= 3)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Здание уже максимального уровня"
            };
        }
        
        if (!BuildingConfig.Buildings.TryGetValue(building.Type, out var info))
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Неизвестный тип здания"
            };
        }
        
        var nextLevel = building.Level + 1;
        if (!info.Levels.TryGetValue(nextLevel, out var levelInfo) || levelInfo.UpgradeCost == null)
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.InvalidAction,
                Message = "Невозможно улучшить это здание"
            };
        }
        
        if (!player.HasResources(levelInfo.UpgradeCost))
        {
            return new ResponseDto
            {
                Success = false,
                ErrorCode = ErrorCode.NotEnoughResources,
                Message = "Недостаточно ресурсов для улучшения"
            };
        }
        
        player.ConsumeResources(levelInfo.UpgradeCost);
        
        // Инженер возвращает 1-2 случайных ресурса
        if (player.Archetype == ArchetypeType.Engineer)
        {
            var refundCount = _random.Next(1, 3);
            var costList = levelInfo.UpgradeCost.ToList();
            for (int i = 0; i < refundCount && costList.Count > 0; i++)
            {
                var randomIndex = _random.Next(costList.Count);
                var (resource, _) = costList[randomIndex];
                player.AddResources(new Dictionary<string, int> { { resource, 1 } });
            }
        }
        
        building.Level = nextLevel;
        
        return new ResponseDto { Success = true };
    }
}
