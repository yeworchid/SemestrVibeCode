using Common;

namespace Server;

public class BuildingInfo
{
    public string Name { get; set; }
    public BuildingCategory Category { get; set; }
    public Dictionary<string, int> BuildCost { get; set; }
    public Dictionary<int, BuildingLevelInfo> Levels { get; set; }
}

public class BuildingLevelInfo
{
    public Dictionary<string, int>? UpgradeCost { get; set; }
    public Dictionary<string, int>? Input { get; set; }
    public Dictionary<string, int>? Output { get; set; }
    public int? DefenseBonus { get; set; }
}

public enum BuildingCategory
{
    Producer,
    Processor,
    Barracks,
    Defense,
    Precious
}

public static class BuildingConfig
{
    public static readonly Dictionary<BuildingType, BuildingInfo> Buildings = new()
    {
        // ПРОИЗВОДИТЕЛИ
        {
            BuildingType.Logging, new BuildingInfo
            {
                Name = "Лесозаготовка",
                Category = BuildingCategory.Producer,
                BuildCost = new() { { "Stone", 2 } },
                Levels = new()
                {
                    { 1, new() { Output = new() { { "Wood", 2 } } } },
                    { 2, new() { UpgradeCost = new() { { "Stone", 1 }, { "Lumber", 1 } }, Output = new() { { "Wood", 3 } } } },
                    { 3, new() { UpgradeCost = new() { { "Lumber", 2 }, { "Bricks", 1 } }, Output = new() { { "Wood", 6 } } } }
                }
            }
        },
        {
            BuildingType.Quarry, new BuildingInfo
            {
                Name = "Карьер",
                Category = BuildingCategory.Producer,
                BuildCost = new() { { "Wood", 2 } },
                Levels = new()
                {
                    { 1, new() { Output = new() { { "Stone", 2 } } } },
                    { 2, new() { UpgradeCost = new() { { "Wood", 1 }, { "Bricks", 1 } }, Output = new() { { "Stone", 3 } } } },
                    { 3, new() { UpgradeCost = new() { { "Bricks", 2 }, { "Lumber", 1 } }, Output = new() { { "Stone", 6 } } } }
                }
            }
        },
        {
            BuildingType.Mine, new BuildingInfo
            {
                Name = "Шахта",
                Category = BuildingCategory.Producer,
                BuildCost = new() { { "Wood", 2 }, { "Stone", 1 } },
                Levels = new()
                {
                    { 1, new() { Output = new() { { "Ore", 2 } } } },
                    { 2, new() { UpgradeCost = new() { { "Bricks", 2 } }, Output = new() { { "Ore", 3 } } } },
                    { 3, new() { UpgradeCost = new() { { "Walls", 1 }, { "Tools", 1 } }, Output = new() { { "Ore", 6 } } } }
                }
            }
        },
        {
            BuildingType.Farm, new BuildingInfo
            {
                Name = "Поле",
                Category = BuildingCategory.Producer,
                BuildCost = new() { { "Wood", 2 } },
                Levels = new()
                {
                    { 1, new() { Output = new() { { "Wheat", 2 } } } },
                    { 2, new() { UpgradeCost = new() { { "Wood", 1 }, { "Lumber", 1 } }, Output = new() { { "Wheat", 3 } } } },
                    { 3, new() { UpgradeCost = new() { { "Lumber", 2 }, { "Bread", 1 } }, Output = new() { { "Wheat", 6 } } } }
                }
            }
        },
        
        // ПЕРЕРАБОТЧИКИ КЛАСС 2
        {
            BuildingType.Sawmill, new BuildingInfo
            {
                Name = "Лесопилка",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Wood", 2 }, { "Stone", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Wood", 2 } }, Output = new() { { "Lumber", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Lumber", 2 } }, Input = new() { { "Wood", 2 } }, Output = new() { { "Lumber", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Bricks", 1 }, { "Lumber", 2 } }, Input = new() { { "Wood", 2 } }, Output = new() { { "Lumber", 4 } } } }
                }
            }
        },
        {
            BuildingType.KilnFurnace, new BuildingInfo
            {
                Name = "Печь обжига",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Stone", 2 }, { "Wood", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Stone", 2 } }, Output = new() { { "Bricks", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Bricks", 2 } }, Input = new() { { "Stone", 2 } }, Output = new() { { "Bricks", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Walls", 1 }, { "Lumber", 1 } }, Input = new() { { "Stone", 2 } }, Output = new() { { "Bricks", 4 } } } }
                }
            }
        },
        {
            BuildingType.Smelter, new BuildingInfo
            {
                Name = "Плавильня",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Stone", 2 }, { "Ore", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Ore", 3 } }, Output = new() { { "Metal", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Metal", 1 }, { "Bricks", 1 } }, Input = new() { { "Ore", 3 } }, Output = new() { { "Metal", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Metal", 2 }, { "Walls", 1 } }, Input = new() { { "Ore", 3 } }, Output = new() { { "Metal", 4 } } } }
                }
            }
        },
        {
            BuildingType.Charcoal, new BuildingInfo
            {
                Name = "Углежог",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Wood", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Wood", 2 } }, Output = new() { { "Coal", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Lumber", 1 }, { "Wood", 1 } }, Input = new() { { "Wood", 2 } }, Output = new() { { "Coal", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Lumber", 2 }, { "Coal", 1 } }, Input = new() { { "Wood", 2 } }, Output = new() { { "Coal", 4 } } } }
                }
            }
        },
        {
            BuildingType.Crusher, new BuildingInfo
            {
                Name = "Дробилка",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Stone", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Stone", 2 } }, Output = new() { { "Sand", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Bricks", 1 }, { "Stone", 1 } }, Input = new() { { "Stone", 2 } }, Output = new() { { "Sand", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Bricks", 2 }, { "Sand", 1 } }, Input = new() { { "Stone", 2 } }, Output = new() { { "Sand", 4 } } } }
                }
            }
        },
        {
            BuildingType.Bakery, new BuildingInfo
            {
                Name = "Пекарня",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Lumber", 2 }, { "Stone", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Wheat", 2 }, { "Wood", 1 } }, Output = new() { { "Bread", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Bread", 1 }, { "Lumber", 1 } }, Input = new() { { "Wheat", 2 }, { "Wood", 1 } }, Output = new() { { "Bread", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Bread", 2 }, { "Bricks", 1 } }, Input = new() { { "Wheat", 2 }, { "Wood", 1 } }, Output = new() { { "Bread", 4 } } } }
                }
            }
        },
        
        // ПЕРЕРАБОТЧИКИ КЛАСС 3
        {
            BuildingType.Carpentry, new BuildingInfo
            {
                Name = "Столярная",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Lumber", 3 }, { "Bricks", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Lumber", 3 } }, Output = new() { { "Furniture", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Furniture", 2 } }, Input = new() { { "Lumber", 3 } }, Output = new() { { "Furniture", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Furniture", 1 }, { "Walls", 1 } }, Input = new() { { "Lumber", 3 } }, Output = new() { { "Furniture", 3 } } } }
                }
            }
        },
        {
            BuildingType.Stonemason, new BuildingInfo
            {
                Name = "Каменотёс",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Bricks", 2 }, { "Stone", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Bricks", 2 }, { "Stone", 1 } }, Output = new() { { "Walls", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Walls", 1 }, { "Bricks", 1 } }, Input = new() { { "Bricks", 2 }, { "Stone", 1 } }, Output = new() { { "Walls", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Walls", 2 }, { "Tools", 1 } }, Input = new() { { "Bricks", 2 }, { "Stone", 1 } }, Output = new() { { "Walls", 3 } } } }
                }
            }
        },
        {
            BuildingType.Forge, new BuildingInfo
            {
                Name = "Кузница",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Metal", 2 }, { "Lumber", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Metal", 2 }, { "Wood", 1 } }, Output = new() { { "Tools", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Tools", 1 }, { "Metal", 1 } }, Input = new() { { "Metal", 2 }, { "Wood", 1 } }, Output = new() { { "Tools", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Tools", 2 }, { "Walls", 1 } }, Input = new() { { "Metal", 2 }, { "Wood", 1 } }, Output = new() { { "Tools", 3 } } } }
                }
            }
        },
        {
            BuildingType.Glassworks, new BuildingInfo
            {
                Name = "Стекловарня",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Sand", 2 }, { "Coal", 1 }, { "Bricks", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Sand", 2 }, { "Coal", 1 } }, Output = new() { { "Glass", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Glass", 1 }, { "Sand", 1 } }, Input = new() { { "Sand", 2 }, { "Coal", 1 } }, Output = new() { { "Glass", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Glass", 2 }, { "Tools", 1 } }, Input = new() { { "Sand", 2 }, { "Coal", 1 } }, Output = new() { { "Glass", 3 } } } }
                }
            }
        },
        {
            BuildingType.Armory, new BuildingInfo
            {
                Name = "Оружейная",
                Category = BuildingCategory.Processor,
                BuildCost = new() { { "Metal", 2 }, { "Lumber", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Metal", 2 }, { "Lumber", 1 } }, Output = new() { { "Weapon", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Weapon", 1 }, { "Metal", 1 } }, Input = new() { { "Metal", 2 }, { "Lumber", 1 } }, Output = new() { { "Weapon", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Weapon", 2 }, { "Tools", 1 } }, Input = new() { { "Metal", 2 }, { "Lumber", 1 } }, Output = new() { { "Weapon", 3 } } } }
                }
            }
        },
        
        // ДРАГОЦЕННОСТИ
        {
            BuildingType.Laboratory, new BuildingInfo
            {
                Name = "Лаборатория",
                Category = BuildingCategory.Precious,
                BuildCost = new() { { "Glass", 2 }, { "Tools", 2 }, { "Coal", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Glass", 2 }, { "Coal", 1 }, { "Tools", 1 } }, Output = new() { { "Emerald", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Emerald", 1 }, { "Glass", 1 } }, Input = new() { { "Glass", 2 }, { "Coal", 1 }, { "Tools", 1 } }, Output = new() { { "Emerald", 1 } } } },
                    { 3, new() { UpgradeCost = new() { { "Emerald", 2 }, { "Tools", 1 } }, Input = new() { { "Glass", 2 }, { "Coal", 1 }, { "Tools", 1 } }, Output = new() { { "Emerald", 2 } } } }
                }
            }
        },
        {
            BuildingType.AlchemicalFurnace, new BuildingInfo
            {
                Name = "Алхимическая печь",
                Category = BuildingCategory.Precious,
                BuildCost = new() { { "Metal", 2 }, { "Coal", 2 }, { "Tools", 2 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Metal", 2 }, { "Coal", 2 }, { "Tools", 1 } }, Output = new() { { "Gold", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Gold", 1 }, { "Metal", 1 } }, Input = new() { { "Metal", 2 }, { "Coal", 2 }, { "Tools", 1 } }, Output = new() { { "Gold", 1 } } } },
                    { 3, new() { UpgradeCost = new() { { "Gold", 2 }, { "Tools", 1 } }, Input = new() { { "Metal", 2 }, { "Coal", 2 }, { "Tools", 1 } }, Output = new() { { "Gold", 2 } } } }
                }
            }
        },
        
        // КАЗАРМЫ
        {
            BuildingType.Barracks, new BuildingInfo
            {
                Name = "Казармы",
                Category = BuildingCategory.Barracks,
                BuildCost = new() { { "Lumber", 2 }, { "Weapon", 1 } },
                Levels = new()
                {
                    { 1, new() { Input = new() { { "Bread", 3 }, { "Weapon", 1 } }, Output = new() { { "Soldier", 1 } } } },
                    { 2, new() { UpgradeCost = new() { { "Walls", 1 }, { "Bread", 1 } }, Input = new() { { "Bread", 3 }, { "Weapon", 1 } }, Output = new() { { "Soldier", 2 } } } },
                    { 3, new() { UpgradeCost = new() { { "Weapon", 1 }, { "Walls", 1 }, { "Bread", 1 } }, Input = new() { { "Bread", 3 }, { "Weapon", 1 } }, Output = new() { { "Soldier", 3 } } } }
                }
            }
        },
        
        // ОБОРОНА
        {
            BuildingType.Barricade, new BuildingInfo
            {
                Name = "Баррикада",
                Category = BuildingCategory.Defense,
                BuildCost = new() { { "Lumber", 2 }, { "Stone", 1 } },
                Levels = new()
                {
                    { 1, new() { DefenseBonus = 5 } },
                    { 2, new() { UpgradeCost = new() { { "Metal", 1 } }, DefenseBonus = 10 } },
                    { 3, new() { DefenseBonus = 10 } }
                }
            }
        },
        {
            BuildingType.DefenseTower, new BuildingInfo
            {
                Name = "Оборонительная башня",
                Category = BuildingCategory.Defense,
                BuildCost = new() { { "Walls", 1 }, { "Tools", 1 }, { "Weapon", 1 } },
                Levels = new()
                {
                    { 1, new() { DefenseBonus = 20 } },
                    { 2, new() { UpgradeCost = new() { { "Weapon", 3 } }, DefenseBonus = 30 } },
                    { 3, new() { DefenseBonus = 30 } }
                }
            }
        }
    };
}
