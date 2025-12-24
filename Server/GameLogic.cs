using Common;

namespace Server;

public class GameLogic
{
    public static Dictionary<Resources, int> GetStartResources()
    {
        var res = new Dictionary<Resources, int>();
        res[Resources.Wood] = 10;
        res[Resources.Stone] = 10;
        res[Resources.Ore] = 5;
        res[Resources.Wheat] = 8;
        return res;
    }

    public static int GetResourcePoints(Resources resource)
    {
        switch (resource)
        {
            case Resources.Wood:
            case Resources.Stone:
            case Resources.Ore:
            case Resources.Wheat:
                return 1;
            case Resources.Lumber:
            case Resources.Bricks:
            case Resources.Metal:
            case Resources.Coal:
            case Resources.Sand:
            case Resources.Bread:
                return 3;
            case Resources.Furniture:
            case Resources.Walls:
            case Resources.Tools:
            case Resources.Glass:
            case Resources.Weapon:
                return 8;
            case Resources.Gold:
            case Resources.Emerald:
                return 25;
            default:
                return 0;
        }
    }

    public static Dictionary<Resources, int> GetBuildCost(BuildingType type)
    {
        var cost = new Dictionary<Resources, int>();
        switch (type)
        {
            case BuildingType.Logging:
                cost[Resources.Stone] = 2;
                break;
            case BuildingType.Quarry:
                cost[Resources.Wood] = 2;
                break;
            case BuildingType.Mine:
                cost[Resources.Wood] = 2;
                cost[Resources.Stone] = 1;
                break;
            case BuildingType.Farm:
                cost[Resources.Wood] = 2;
                break;
            case BuildingType.Sawmill:
                cost[Resources.Wood] = 2;
                cost[Resources.Stone] = 1;
                break;
            case BuildingType.Kiln:
                cost[Resources.Stone] = 2;
                cost[Resources.Wood] = 1;
                break;
            case BuildingType.Smelter:
                cost[Resources.Stone] = 2;
                cost[Resources.Ore] = 2;
                break;
            case BuildingType.Charcoal:
                cost[Resources.Wood] = 2;
                break;
            case BuildingType.Crusher:
                cost[Resources.Stone] = 2;
                break;
            case BuildingType.Bakery:
                cost[Resources.Lumber] = 2;
                cost[Resources.Stone] = 1;
                break;
            case BuildingType.Carpentry:
                cost[Resources.Lumber] = 3;
                cost[Resources.Bricks] = 1;
                break;
            case BuildingType.Masonry:
                cost[Resources.Bricks] = 2;
                cost[Resources.Stone] = 2;
                break;
            case BuildingType.Forge:
                cost[Resources.Metal] = 2;
                cost[Resources.Lumber] = 2;
                break;
            case BuildingType.Glassworks:
                cost[Resources.Sand] = 2;
                cost[Resources.Coal] = 1;
                cost[Resources.Bricks] = 1;
                break;
            case BuildingType.Armory:
                cost[Resources.Metal] = 2;
                cost[Resources.Lumber] = 2;
                break;
            case BuildingType.Barracks:
                cost[Resources.Lumber] = 2;
                cost[Resources.Weapon] = 1;
                break;
            case BuildingType.Laboratory:
                cost[Resources.Glass] = 2;
                cost[Resources.Tools] = 2;
                cost[Resources.Coal] = 1;
                break;
            case BuildingType.AlchemyFurnace:
                cost[Resources.Metal] = 2;
                cost[Resources.Coal] = 2;
                cost[Resources.Tools] = 2;
                break;
            case BuildingType.Barricade:
                cost[Resources.Lumber] = 2;
                cost[Resources.Stone] = 1;
                break;
            case BuildingType.DefenseTower:
                cost[Resources.Walls] = 1;
                cost[Resources.Tools] = 1;
                cost[Resources.Weapon] = 1;
                break;
        }
        return cost;
    }

    public static Dictionary<Resources, int> GetUpgradeCost(BuildingType type, int level)
    {
        var cost = new Dictionary<Resources, int>();
        if (level == 2)
        {
            switch (type)
            {
                case BuildingType.Logging:
                    cost[Resources.Stone] = 1;
                    cost[Resources.Lumber] = 1;
                    break;
                case BuildingType.Quarry:
                    cost[Resources.Wood] = 1;
                    cost[Resources.Bricks] = 1;
                    break;
                case BuildingType.Mine:
                    cost[Resources.Bricks] = 2;
                    break;
                case BuildingType.Farm:
                    cost[Resources.Wood] = 1;
                    cost[Resources.Lumber] = 1;
                    break;
                case BuildingType.Sawmill:
                    cost[Resources.Lumber] = 2;
                    break;
                case BuildingType.Kiln:
                    cost[Resources.Bricks] = 2;
                    break;
                case BuildingType.Smelter:
                    cost[Resources.Metal] = 1;
                    cost[Resources.Bricks] = 1;
                    break;
                case BuildingType.Charcoal:
                    cost[Resources.Lumber] = 1;
                    cost[Resources.Wood] = 1;
                    break;
                case BuildingType.Crusher:
                    cost[Resources.Bricks] = 1;
                    cost[Resources.Stone] = 1;
                    break;
                case BuildingType.Bakery:
                    cost[Resources.Bread] = 1;
                    cost[Resources.Lumber] = 1;
                    break;
                case BuildingType.Carpentry:
                    cost[Resources.Furniture] = 2;
                    break;
                case BuildingType.Masonry:
                    cost[Resources.Walls] = 1;
                    cost[Resources.Bricks] = 1;
                    break;
                case BuildingType.Forge:
                    cost[Resources.Tools] = 1;
                    cost[Resources.Metal] = 1;
                    break;
                case BuildingType.Glassworks:
                    cost[Resources.Glass] = 1;
                    cost[Resources.Sand] = 1;
                    break;
                case BuildingType.Armory:
                    cost[Resources.Weapon] = 1;
                    cost[Resources.Metal] = 1;
                    break;
                case BuildingType.Barracks:
                    cost[Resources.Walls] = 1;
                    cost[Resources.Bread] = 1;
                    break;
                case BuildingType.Laboratory:
                    cost[Resources.Emerald] = 1;
                    cost[Resources.Glass] = 1;
                    break;
                case BuildingType.AlchemyFurnace:
                    cost[Resources.Gold] = 1;
                    cost[Resources.Metal] = 1;
                    break;
                case BuildingType.Barricade:
                    cost[Resources.Metal] = 1;
                    break;
                case BuildingType.DefenseTower:
                    cost[Resources.Weapon] = 3;
                    break;
            }
        }
        else if (level == 3)
        {
            switch (type)
            {
                case BuildingType.Logging:
                    cost[Resources.Lumber] = 2;
                    cost[Resources.Bricks] = 1;
                    break;
                case BuildingType.Quarry:
                    cost[Resources.Bricks] = 2;
                    cost[Resources.Lumber] = 1;
                    break;
                case BuildingType.Mine:
                    cost[Resources.Walls] = 1;
                    cost[Resources.Tools] = 1;
                    break;
                case BuildingType.Farm:
                    cost[Resources.Lumber] = 2;
                    cost[Resources.Bread] = 1;
                    break;
                case BuildingType.Sawmill:
                    cost[Resources.Bricks] = 1;
                    cost[Resources.Lumber] = 2;
                    break;
                case BuildingType.Kiln:
                    cost[Resources.Walls] = 1;
                    cost[Resources.Lumber] = 1;
                    break;
                case BuildingType.Smelter:
                    cost[Resources.Metal] = 2;
                    cost[Resources.Walls] = 1;
                    break;
                case BuildingType.Charcoal:
                    cost[Resources.Lumber] = 2;
                    cost[Resources.Coal] = 1;
                    break;
                case BuildingType.Crusher:
                    cost[Resources.Bricks] = 2;
                    cost[Resources.Sand] = 1;
                    break;
                case BuildingType.Bakery:
                    cost[Resources.Bread] = 2;
                    cost[Resources.Bricks] = 1;
                    break;
                case BuildingType.Carpentry:
                    cost[Resources.Furniture] = 1;
                    cost[Resources.Walls] = 1;
                    break;
                case BuildingType.Masonry:
                    cost[Resources.Walls] = 2;
                    cost[Resources.Tools] = 1;
                    break;
                case BuildingType.Forge:
                    cost[Resources.Tools] = 2;
                    cost[Resources.Walls] = 1;
                    break;
                case BuildingType.Glassworks:
                    cost[Resources.Glass] = 2;
                    cost[Resources.Tools] = 1;
                    break;
                case BuildingType.Armory:
                    cost[Resources.Weapon] = 2;
                    cost[Resources.Tools] = 1;
                    break;
                case BuildingType.Barracks:
                    cost[Resources.Weapon] = 1;
                    cost[Resources.Walls] = 1;
                    cost[Resources.Bread] = 1;
                    break;
                case BuildingType.Laboratory:
                    cost[Resources.Emerald] = 2;
                    cost[Resources.Tools] = 1;
                    break;
                case BuildingType.AlchemyFurnace:
                    cost[Resources.Gold] = 2;
                    cost[Resources.Tools] = 1;
                    break;
            }
        }
        return cost;
    }

    public static int GetProduction(BuildingType type, int level)
    {
        if (type == BuildingType.Logging || type == BuildingType.Quarry || type == BuildingType.Mine || type == BuildingType.Farm)
        {
            if (level == 1) return 2;
            if (level == 2) return 3;
            if (level == 3) return 6;
        }
        else if (type == BuildingType.Sawmill || type == BuildingType.Kiln || type == BuildingType.Smelter || 
                 type == BuildingType.Charcoal || type == BuildingType.Crusher || type == BuildingType.Bakery)
        {
            if (level == 1) return 1;
            if (level == 2) return 2;
            if (level == 3) return 4;
        }
        else if (type == BuildingType.Carpentry || type == BuildingType.Masonry || type == BuildingType.Forge || 
                 type == BuildingType.Glassworks || type == BuildingType.Armory)
        {
            if (level == 1) return 1;
            if (level == 2) return 2;
            if (level == 3) return 3;
        }
        else if (type == BuildingType.Barracks)
        {
            if (level == 1) return 1;
            if (level == 2) return 2;
            if (level == 3) return 3;
        }
        else if (type == BuildingType.Laboratory || type == BuildingType.AlchemyFurnace)
        {
            if (level == 1) return 1;
            if (level == 2) return 1;
            if (level == 3) return 2;
        }
        return 0;
    }

    public static int GetDefenseBonus(BuildingType type, int level)
    {
        if (type == BuildingType.Barricade)
        {
            if (level == 1) return 5;
            if (level == 2) return 10;
        }
        else if (type == BuildingType.DefenseTower)
        {
            if (level == 1) return 20;
            if (level == 2) return 30;
        }
        return 0;
    }

    public static Dictionary<Resources, int> GetProcessInput(BuildingType type)
    {
        var input = new Dictionary<Resources, int>();
        switch (type)
        {
            case BuildingType.Sawmill:
                input[Resources.Wood] = 2;
                break;
            case BuildingType.Kiln:
                input[Resources.Stone] = 2;
                break;
            case BuildingType.Smelter:
                input[Resources.Ore] = 3;
                break;
            case BuildingType.Charcoal:
                input[Resources.Wood] = 2;
                break;
            case BuildingType.Crusher:
                input[Resources.Stone] = 2;
                break;
            case BuildingType.Bakery:
                input[Resources.Wheat] = 2;
                input[Resources.Wood] = 1;
                break;
            case BuildingType.Carpentry:
                input[Resources.Lumber] = 3;
                break;
            case BuildingType.Masonry:
                input[Resources.Bricks] = 2;
                input[Resources.Stone] = 1;
                break;
            case BuildingType.Forge:
                input[Resources.Metal] = 2;
                input[Resources.Wood] = 1;
                break;
            case BuildingType.Glassworks:
                input[Resources.Sand] = 2;
                input[Resources.Coal] = 1;
                break;
            case BuildingType.Armory:
                input[Resources.Metal] = 2;
                input[Resources.Lumber] = 1;
                break;
            case BuildingType.Laboratory:
                input[Resources.Glass] = 2;
                input[Resources.Coal] = 1;
                input[Resources.Tools] = 1;
                break;
            case BuildingType.AlchemyFurnace:
                input[Resources.Metal] = 2;
                input[Resources.Coal] = 2;
                input[Resources.Tools] = 1;
                break;
        }
        return input;
    }

    public static Resources GetProcessOutput(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Sawmill: return Resources.Lumber;
            case BuildingType.Kiln: return Resources.Bricks;
            case BuildingType.Smelter: return Resources.Metal;
            case BuildingType.Charcoal: return Resources.Coal;
            case BuildingType.Crusher: return Resources.Sand;
            case BuildingType.Bakery: return Resources.Bread;
            case BuildingType.Carpentry: return Resources.Furniture;
            case BuildingType.Masonry: return Resources.Walls;
            case BuildingType.Forge: return Resources.Tools;
            case BuildingType.Glassworks: return Resources.Glass;
            case BuildingType.Armory: return Resources.Weapon;
            case BuildingType.Laboratory: return Resources.Emerald;
            case BuildingType.AlchemyFurnace: return Resources.Gold;
            default: return Resources.Wood;
        }
    }

    public static Resources GetProducerOutput(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Logging: return Resources.Wood;
            case BuildingType.Quarry: return Resources.Stone;
            case BuildingType.Mine: return Resources.Ore;
            case BuildingType.Farm: return Resources.Wheat;
            default: return Resources.Wood;
        }
    }

    public static bool IsProducer(BuildingType type)
    {
        return type == BuildingType.Logging || type == BuildingType.Quarry || 
               type == BuildingType.Mine || type == BuildingType.Farm;
    }

    public static bool IsProcessor(BuildingType type)
    {
        return type == BuildingType.Sawmill || type == BuildingType.Kiln || type == BuildingType.Smelter ||
               type == BuildingType.Charcoal || type == BuildingType.Crusher || type == BuildingType.Bakery ||
               type == BuildingType.Carpentry || type == BuildingType.Masonry || type == BuildingType.Forge ||
               type == BuildingType.Glassworks || type == BuildingType.Armory || type == BuildingType.Laboratory ||
               type == BuildingType.AlchemyFurnace;
    }

    public static Dictionary<Resources, int> GetSoldierCost(ArchetypeType archetype)
    {
        var cost = new Dictionary<Resources, int>();
        if (archetype == ArchetypeType.Warrior)
        {
            cost[Resources.Bread] = 5;
            cost[Resources.Weapon] = 2;
        }
        else if (archetype == ArchetypeType.Recruit)
        {
            cost[Resources.Bread] = 1;
            cost[Resources.Weapon] = 1;
        }
        else if (archetype == ArchetypeType.Glutton)
        {
            cost[Resources.Bread] = 6;
            cost[Resources.Weapon] = 1;
        }
        else
        {
            cost[Resources.Bread] = 3;
            cost[Resources.Weapon] = 1;
        }
        return cost;
    }
}
