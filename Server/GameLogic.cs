using Common;

namespace Server;

public static class GameLogic
{
    public static Dictionary<Resources, int> GetBuildCost(BuildingType type)
    {
        var c = new Dictionary<Resources, int>();
        
        if (type == BuildingType.Logging) { c[Resources.Stone] = 2; }
        else if (type == BuildingType.Quarry) { c[Resources.Wood] = 2; }
        else if (type == BuildingType.Mine) { c[Resources.Wood] = 2; c[Resources.Stone] = 1; }
        else if (type == BuildingType.Farm) { c[Resources.Wood] = 2; }
        else if (type == BuildingType.Sawmill) { c[Resources.Wood] = 2; c[Resources.Stone] = 1; }
        else if (type == BuildingType.Kiln) { c[Resources.Stone] = 2; c[Resources.Wood] = 1; }
        else if (type == BuildingType.Smelter) { c[Resources.Stone] = 2; c[Resources.Ore] = 2; }
        else if (type == BuildingType.Charcoal) { c[Resources.Wood] = 2; }
        else if (type == BuildingType.Crusher) { c[Resources.Stone] = 2; }
        else if (type == BuildingType.Bakery) { c[Resources.Lumber] = 2; c[Resources.Stone] = 1; }
        else if (type == BuildingType.Carpentry) { c[Resources.Lumber] = 3; c[Resources.Bricks] = 1; }
        else if (type == BuildingType.Masonry) { c[Resources.Bricks] = 2; c[Resources.Stone] = 2; }
        else if (type == BuildingType.Forge) { c[Resources.Metal] = 2; c[Resources.Lumber] = 2; }
        else if (type == BuildingType.Glassworks) { c[Resources.Sand] = 2; c[Resources.Coal] = 1; c[Resources.Bricks] = 1; }
        else if (type == BuildingType.Armory) { c[Resources.Metal] = 2; c[Resources.Lumber] = 2; }
        else if (type == BuildingType.Barracks) { c[Resources.Lumber] = 2; c[Resources.Weapon] = 1; }
        else if (type == BuildingType.Laboratory) { c[Resources.Glass] = 2; c[Resources.Tools] = 2; c[Resources.Coal] = 1; }
        else if (type == BuildingType.AlchemyFurnace) { c[Resources.Metal] = 2; c[Resources.Coal] = 2; c[Resources.Tools] = 2; }
        else if (type == BuildingType.Barricade) { c[Resources.Lumber] = 2; c[Resources.Stone] = 1; }
        else if (type == BuildingType.DefenseTower) { c[Resources.Walls] = 1; c[Resources.Tools] = 1; c[Resources.Weapon] = 1; }
        
        return c;
    }

    public static Dictionary<Resources, int> GetUpgradeCost(BuildingType type, int toLevel)
    {
        var c = new Dictionary<Resources, int>();
        
        if (toLevel == 2)
        {
            if (type == BuildingType.Logging) { c[Resources.Stone] = 1; c[Resources.Lumber] = 1; }
            else if (type == BuildingType.Quarry) { c[Resources.Wood] = 1; c[Resources.Bricks] = 1; }
            else if (type == BuildingType.Mine) { c[Resources.Bricks] = 2; }
            else if (type == BuildingType.Farm) { c[Resources.Wood] = 1; c[Resources.Lumber] = 1; }
            else if (type == BuildingType.Sawmill) { c[Resources.Lumber] = 2; }
            else if (type == BuildingType.Kiln) { c[Resources.Bricks] = 2; }
            else if (type == BuildingType.Smelter) { c[Resources.Metal] = 1; c[Resources.Bricks] = 1; }
            else if (type == BuildingType.Charcoal) { c[Resources.Lumber] = 1; c[Resources.Wood] = 1; }
            else if (type == BuildingType.Crusher) { c[Resources.Bricks] = 1; c[Resources.Stone] = 1; }
            else if (type == BuildingType.Bakery) { c[Resources.Bread] = 1; c[Resources.Lumber] = 1; }
            else if (type == BuildingType.Carpentry) { c[Resources.Furniture] = 2; }
            else if (type == BuildingType.Masonry) { c[Resources.Walls] = 1; c[Resources.Bricks] = 1; }
            else if (type == BuildingType.Forge) { c[Resources.Tools] = 1; c[Resources.Metal] = 1; }
            else if (type == BuildingType.Glassworks) { c[Resources.Glass] = 1; c[Resources.Sand] = 1; }
            else if (type == BuildingType.Armory) { c[Resources.Weapon] = 1; c[Resources.Metal] = 1; }
            else if (type == BuildingType.Barracks) { c[Resources.Walls] = 1; c[Resources.Bread] = 1; }
            else if (type == BuildingType.Laboratory) { c[Resources.Emerald] = 1; c[Resources.Glass] = 1; }
            else if (type == BuildingType.AlchemyFurnace) { c[Resources.Gold] = 1; c[Resources.Metal] = 1; }
            else if (type == BuildingType.Barricade) { c[Resources.Metal] = 1; }
            else if (type == BuildingType.DefenseTower) { c[Resources.Weapon] = 3; }
        }
        else if (toLevel == 3)
        {
            if (type == BuildingType.Logging) { c[Resources.Lumber] = 2; c[Resources.Bricks] = 1; }
            else if (type == BuildingType.Quarry) { c[Resources.Bricks] = 2; c[Resources.Lumber] = 1; }
            else if (type == BuildingType.Mine) { c[Resources.Walls] = 1; c[Resources.Tools] = 1; }
            else if (type == BuildingType.Farm) { c[Resources.Lumber] = 2; c[Resources.Bread] = 1; }
            else if (type == BuildingType.Sawmill) { c[Resources.Bricks] = 1; c[Resources.Lumber] = 2; }
            else if (type == BuildingType.Kiln) { c[Resources.Walls] = 1; c[Resources.Lumber] = 1; }
            else if (type == BuildingType.Smelter) { c[Resources.Metal] = 2; c[Resources.Walls] = 1; }
            else if (type == BuildingType.Charcoal) { c[Resources.Lumber] = 2; c[Resources.Coal] = 1; }
            else if (type == BuildingType.Crusher) { c[Resources.Bricks] = 2; c[Resources.Sand] = 1; }
            else if (type == BuildingType.Bakery) { c[Resources.Bread] = 2; c[Resources.Bricks] = 1; }
            else if (type == BuildingType.Carpentry) { c[Resources.Furniture] = 1; c[Resources.Walls] = 1; }
            else if (type == BuildingType.Masonry) { c[Resources.Walls] = 2; c[Resources.Tools] = 1; }
            else if (type == BuildingType.Forge) { c[Resources.Tools] = 2; c[Resources.Walls] = 1; }
            else if (type == BuildingType.Glassworks) { c[Resources.Glass] = 2; c[Resources.Tools] = 1; }
            else if (type == BuildingType.Armory) { c[Resources.Weapon] = 2; c[Resources.Tools] = 1; }
            else if (type == BuildingType.Barracks) { c[Resources.Weapon] = 1; c[Resources.Walls] = 1; c[Resources.Bread] = 1; }
            else if (type == BuildingType.Laboratory) { c[Resources.Emerald] = 2; c[Resources.Tools] = 1; }
            else if (type == BuildingType.AlchemyFurnace) { c[Resources.Gold] = 2; c[Resources.Tools] = 1; }
        }
        
        return c;
    }

    public static int GetProduction(BuildingType type, int level)
    {
        if (type == BuildingType.Logging || type == BuildingType.Quarry || 
            type == BuildingType.Mine || type == BuildingType.Farm)
        {
            if (level == 1) return 2;
            if (level == 2) return 3;
            if (level == 3) return 6;
        }
        
        if (type == BuildingType.Sawmill || type == BuildingType.Kiln || 
            type == BuildingType.Smelter || type == BuildingType.Charcoal || 
            type == BuildingType.Crusher || type == BuildingType.Bakery)
        {
            if (level == 1) return 1;
            if (level == 2) return 2;
            if (level == 3) return 4;
        }
        
        if (type == BuildingType.Carpentry || type == BuildingType.Masonry || 
            type == BuildingType.Forge || type == BuildingType.Glassworks || 
            type == BuildingType.Armory)
        {
            if (level == 1) return 1;
            if (level == 2) return 2;
            if (level == 3) return 3;
        }
        
        if (type == BuildingType.Barracks)
        {
            if (level == 1) return 1;
            if (level == 2) return 2;
            if (level == 3) return 3;
        }
        
        if (type == BuildingType.Laboratory || type == BuildingType.AlchemyFurnace)
        {
            if (level == 1) return 1;
            if (level == 2) return 1;
            if (level == 3) return 2;
        }
        
        return 0;
    }

    public static Resources GetProducerOutput(BuildingType type)
    {
        if (type == BuildingType.Logging) return Resources.Wood;
        if (type == BuildingType.Quarry) return Resources.Stone;
        if (type == BuildingType.Mine) return Resources.Ore;
        if (type == BuildingType.Farm) return Resources.Wheat;
        return Resources.Wood; // default
    }

    public static Resources GetProcessorOutput(BuildingType type)
    {
        if (type == BuildingType.Sawmill) return Resources.Lumber;
        if (type == BuildingType.Kiln) return Resources.Bricks;
        if (type == BuildingType.Smelter) return Resources.Metal;
        if (type == BuildingType.Charcoal) return Resources.Coal;
        if (type == BuildingType.Crusher) return Resources.Sand;
        if (type == BuildingType.Bakery) return Resources.Bread;
        if (type == BuildingType.Carpentry) return Resources.Furniture;
        if (type == BuildingType.Masonry) return Resources.Walls;
        if (type == BuildingType.Forge) return Resources.Tools;
        if (type == BuildingType.Glassworks) return Resources.Glass;
        if (type == BuildingType.Armory) return Resources.Weapon;
        if (type == BuildingType.Laboratory) return Resources.Emerald;
        if (type == BuildingType.AlchemyFurnace) return Resources.Gold;
        return Resources.Wood; // default
    }

    public static Dictionary<Resources, int> GetProcessorInput(BuildingType type)
    {
        var inp = new Dictionary<Resources, int>();
        
        if (type == BuildingType.Sawmill) { inp[Resources.Wood] = 2; }
        else if (type == BuildingType.Kiln) { inp[Resources.Stone] = 2; }
        else if (type == BuildingType.Smelter) { inp[Resources.Ore] = 3; }
        else if (type == BuildingType.Charcoal) { inp[Resources.Wood] = 2; }
        else if (type == BuildingType.Crusher) { inp[Resources.Stone] = 2; }
        else if (type == BuildingType.Bakery) { inp[Resources.Wheat] = 2; inp[Resources.Wood] = 1; }
        else if (type == BuildingType.Carpentry) { inp[Resources.Lumber] = 3; }
        else if (type == BuildingType.Masonry) { inp[Resources.Bricks] = 2; inp[Resources.Stone] = 1; }
        else if (type == BuildingType.Forge) { inp[Resources.Metal] = 2; inp[Resources.Wood] = 1; }
        else if (type == BuildingType.Glassworks) { inp[Resources.Sand] = 2; inp[Resources.Coal] = 1; }
        else if (type == BuildingType.Armory) { inp[Resources.Metal] = 2; inp[Resources.Lumber] = 1; }
        else if (type == BuildingType.Laboratory) { inp[Resources.Glass] = 2; inp[Resources.Coal] = 1; inp[Resources.Tools] = 1; }
        else if (type == BuildingType.AlchemyFurnace) { inp[Resources.Metal] = 2; inp[Resources.Coal] = 2; inp[Resources.Tools] = 1; }
        
        return inp;
    }

    public static bool IsProducer(BuildingType type)
    {
        return type == BuildingType.Logging || type == BuildingType.Quarry || 
               type == BuildingType.Mine || type == BuildingType.Farm;
    }

    public static bool IsProcessor(BuildingType type)
    {
        return type == BuildingType.Sawmill || type == BuildingType.Kiln || 
               type == BuildingType.Smelter || type == BuildingType.Charcoal || 
               type == BuildingType.Crusher || type == BuildingType.Bakery ||
               type == BuildingType.Carpentry || type == BuildingType.Masonry || 
               type == BuildingType.Forge || type == BuildingType.Glassworks || 
               type == BuildingType.Armory || type == BuildingType.Laboratory || 
               type == BuildingType.AlchemyFurnace;
    }

    public static Dictionary<Resources, int> GetSoldierCost(ArchetypeType arch)
    {
        var c = new Dictionary<Resources, int>();
        
        if (arch == ArchetypeType.Warrior)
        {
            c[Resources.Bread] = 5;
            c[Resources.Weapon] = 2;
        }
        else if (arch == ArchetypeType.Recruit)
        {
            c[Resources.Bread] = 1;
            c[Resources.Weapon] = 1;
        }
        else if (arch == ArchetypeType.Glutton)
        {
            c[Resources.Bread] = 6;
            c[Resources.Weapon] = 1;
        }
        else
        {
            c[Resources.Bread] = 3;
            c[Resources.Weapon] = 1;
        }
        
        return c;
    }
}
