using Common;

namespace Server;

public static class GameLogic
{
    public static Dictionary<string, int> GetBuildCost(BuildingType type)
    {
        var c = new Dictionary<string, int>();
        
        if (type == BuildingType.Logging) { c["Stone"] = 2; }
        else if (type == BuildingType.Quarry) { c["Wood"] = 2; }
        else if (type == BuildingType.Mine) { c["Wood"] = 2; c["Stone"] = 1; }
        else if (type == BuildingType.Farm) { c["Wood"] = 2; }
        else if (type == BuildingType.Sawmill) { c["Wood"] = 2; c["Stone"] = 1; }
        else if (type == BuildingType.Kiln) { c["Stone"] = 2; c["Wood"] = 1; }
        else if (type == BuildingType.Smelter) { c["Stone"] = 2; c["Ore"] = 2; }
        else if (type == BuildingType.Charcoal) { c["Wood"] = 2; }
        else if (type == BuildingType.Crusher) { c["Stone"] = 2; }
        else if (type == BuildingType.Bakery) { c["Lumber"] = 2; c["Stone"] = 1; }
        else if (type == BuildingType.Carpentry) { c["Lumber"] = 3; c["Bricks"] = 1; }
        else if (type == BuildingType.Masonry) { c["Bricks"] = 2; c["Stone"] = 2; }
        else if (type == BuildingType.Forge) { c["Metal"] = 2; c["Lumber"] = 2; }
        else if (type == BuildingType.Glassworks) { c["Sand"] = 2; c["Coal"] = 1; c["Bricks"] = 1; }
        else if (type == BuildingType.Armory) { c["Metal"] = 2; c["Lumber"] = 2; }
        else if (type == BuildingType.Barracks) { c["Lumber"] = 2; c["Weapon"] = 1; }
        else if (type == BuildingType.Laboratory) { c["Glass"] = 2; c["Tools"] = 2; c["Coal"] = 1; }
        else if (type == BuildingType.AlchemyFurnace) { c["Metal"] = 2; c["Coal"] = 2; c["Tools"] = 2; }
        else if (type == BuildingType.Barricade) { c["Lumber"] = 2; c["Stone"] = 1; }
        else if (type == BuildingType.DefenseTower) { c["Walls"] = 1; c["Tools"] = 1; c["Weapon"] = 1; }
        
        return c;
    }

    public static Dictionary<string, int> GetUpgradeCost(BuildingType type, int toLevel)
    {
        var c = new Dictionary<string, int>();
        
        if (toLevel == 2)
        {
            if (type == BuildingType.Logging) { c["Stone"] = 1; c["Lumber"] = 1; }
            else if (type == BuildingType.Quarry) { c["Wood"] = 1; c["Bricks"] = 1; }
            else if (type == BuildingType.Mine) { c["Bricks"] = 2; }
            else if (type == BuildingType.Farm) { c["Wood"] = 1; c["Lumber"] = 1; }
            else if (type == BuildingType.Sawmill) { c["Lumber"] = 2; }
            else if (type == BuildingType.Kiln) { c["Bricks"] = 2; }
            else if (type == BuildingType.Smelter) { c["Metal"] = 1; c["Bricks"] = 1; }
            else if (type == BuildingType.Charcoal) { c["Lumber"] = 1; c["Wood"] = 1; }
            else if (type == BuildingType.Crusher) { c["Bricks"] = 1; c["Stone"] = 1; }
            else if (type == BuildingType.Bakery) { c["Bread"] = 1; c["Lumber"] = 1; }
            else if (type == BuildingType.Carpentry) { c["Furniture"] = 2; }
            else if (type == BuildingType.Masonry) { c["Walls"] = 1; c["Bricks"] = 1; }
            else if (type == BuildingType.Forge) { c["Tools"] = 1; c["Metal"] = 1; }
            else if (type == BuildingType.Glassworks) { c["Glass"] = 1; c["Sand"] = 1; }
            else if (type == BuildingType.Armory) { c["Weapon"] = 1; c["Metal"] = 1; }
            else if (type == BuildingType.Barracks) { c["Walls"] = 1; c["Bread"] = 1; }
            else if (type == BuildingType.Laboratory) { c["Emerald"] = 1; c["Glass"] = 1; }
            else if (type == BuildingType.AlchemyFurnace) { c["Gold"] = 1; c["Metal"] = 1; }
            else if (type == BuildingType.Barricade) { c["Metal"] = 1; }
            else if (type == BuildingType.DefenseTower) { c["Weapon"] = 3; }
        }
        else if (toLevel == 3)
        {
            if (type == BuildingType.Logging) { c["Lumber"] = 2; c["Bricks"] = 1; }
            else if (type == BuildingType.Quarry) { c["Bricks"] = 2; c["Lumber"] = 1; }
            else if (type == BuildingType.Mine) { c["Walls"] = 1; c["Tools"] = 1; }
            else if (type == BuildingType.Farm) { c["Lumber"] = 2; c["Bread"] = 1; }
            else if (type == BuildingType.Sawmill) { c["Bricks"] = 1; c["Lumber"] = 2; }
            else if (type == BuildingType.Kiln) { c["Walls"] = 1; c["Lumber"] = 1; }
            else if (type == BuildingType.Smelter) { c["Metal"] = 2; c["Walls"] = 1; }
            else if (type == BuildingType.Charcoal) { c["Lumber"] = 2; c["Coal"] = 1; }
            else if (type == BuildingType.Crusher) { c["Bricks"] = 2; c["Sand"] = 1; }
            else if (type == BuildingType.Bakery) { c["Bread"] = 2; c["Bricks"] = 1; }
            else if (type == BuildingType.Carpentry) { c["Furniture"] = 1; c["Walls"] = 1; }
            else if (type == BuildingType.Masonry) { c["Walls"] = 2; c["Tools"] = 1; }
            else if (type == BuildingType.Forge) { c["Tools"] = 2; c["Walls"] = 1; }
            else if (type == BuildingType.Glassworks) { c["Glass"] = 2; c["Tools"] = 1; }
            else if (type == BuildingType.Armory) { c["Weapon"] = 2; c["Tools"] = 1; }
            else if (type == BuildingType.Barracks) { c["Weapon"] = 1; c["Walls"] = 1; c["Bread"] = 1; }
            else if (type == BuildingType.Laboratory) { c["Emerald"] = 2; c["Tools"] = 1; }
            else if (type == BuildingType.AlchemyFurnace) { c["Gold"] = 2; c["Tools"] = 1; }
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

    public static string GetProducerOutput(BuildingType type)
    {
        if (type == BuildingType.Logging) return "Wood";
        if (type == BuildingType.Quarry) return "Stone";
        if (type == BuildingType.Mine) return "Ore";
        if (type == BuildingType.Farm) return "Wheat";
        return "";
    }

    public static string GetProcessorOutput(BuildingType type)
    {
        if (type == BuildingType.Sawmill) return "Lumber";
        if (type == BuildingType.Kiln) return "Bricks";
        if (type == BuildingType.Smelter) return "Metal";
        if (type == BuildingType.Charcoal) return "Coal";
        if (type == BuildingType.Crusher) return "Sand";
        if (type == BuildingType.Bakery) return "Bread";
        if (type == BuildingType.Carpentry) return "Furniture";
        if (type == BuildingType.Masonry) return "Walls";
        if (type == BuildingType.Forge) return "Tools";
        if (type == BuildingType.Glassworks) return "Glass";
        if (type == BuildingType.Armory) return "Weapon";
        if (type == BuildingType.Laboratory) return "Emerald";
        if (type == BuildingType.AlchemyFurnace) return "Gold";
        return "";
    }

    public static Dictionary<string, int> GetProcessorInput(BuildingType type)
    {
        var inp = new Dictionary<string, int>();
        
        if (type == BuildingType.Sawmill) { inp["Wood"] = 2; }
        else if (type == BuildingType.Kiln) { inp["Stone"] = 2; }
        else if (type == BuildingType.Smelter) { inp["Ore"] = 3; }
        else if (type == BuildingType.Charcoal) { inp["Wood"] = 2; }
        else if (type == BuildingType.Crusher) { inp["Stone"] = 2; }
        else if (type == BuildingType.Bakery) { inp["Wheat"] = 2; inp["Wood"] = 1; }
        else if (type == BuildingType.Carpentry) { inp["Lumber"] = 3; }
        else if (type == BuildingType.Masonry) { inp["Bricks"] = 2; inp["Stone"] = 1; }
        else if (type == BuildingType.Forge) { inp["Metal"] = 2; inp["Wood"] = 1; }
        else if (type == BuildingType.Glassworks) { inp["Sand"] = 2; inp["Coal"] = 1; }
        else if (type == BuildingType.Armory) { inp["Metal"] = 2; inp["Lumber"] = 1; }
        else if (type == BuildingType.Laboratory) { inp["Glass"] = 2; inp["Coal"] = 1; inp["Tools"] = 1; }
        else if (type == BuildingType.AlchemyFurnace) { inp["Metal"] = 2; inp["Coal"] = 2; inp["Tools"] = 1; }
        
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

    public static Dictionary<string, int> GetSoldierCost(ArchetypeType arch)
    {
        var c = new Dictionary<string, int>();
        
        if (arch == ArchetypeType.Warrior)
        {
            c["Bread"] = 5;
            c["Weapon"] = 2;
        }
        else if (arch == ArchetypeType.Recruit)
        {
            c["Bread"] = 1;
            c["Weapon"] = 1;
        }
        else if (arch == ArchetypeType.Glutton)
        {
            c["Bread"] = 6;
            c["Weapon"] = 1;
        }
        else
        {
            c["Bread"] = 3;
            c["Weapon"] = 1;
        }
        
        return c;
    }
}
