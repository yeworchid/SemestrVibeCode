namespace Server;

public static class GameConfig
{
    public const int FieldSize = 5;
    public const int MaxPlayers = 4;
    public const int MinPlayers = 2;
    public const int TotalCycles = 15;
    public const int PeaceProtectionCycles = 5;
    
    // Стартовые ресурсы
    public static readonly Dictionary<string, int> StartingResources = new()
    {
        { "Wood", 10 },
        { "Stone", 10 },
        { "Ore", 5 },
        { "Wheat", 8 }
    };
    
    // Очки за ресурсы по классам
    public static readonly Dictionary<string, int> ResourcePoints = new()
    {
        // Класс 1 - базовые
        { "Wood", 1 },
        { "Stone", 1 },
        { "Ore", 1 },
        { "Wheat", 1 },
        // Класс 2 - переработанные базовые
        { "Lumber", 3 },
        { "Bricks", 3 },
        { "Metal", 3 },
        { "Coal", 3 },
        { "Sand", 3 },
        { "Bread", 3 },
        // Класс 3 - продвинутые
        { "Furniture", 8 },
        { "Walls", 8 },
        { "Tools", 8 },
        { "Glass", 8 },
        { "Weapon", 8 },
        // Класс 4 - драгоценности
        { "Emerald", 25 },
        { "Gold", 25 }
    };
}
