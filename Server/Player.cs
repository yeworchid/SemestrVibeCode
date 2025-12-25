using Common;
using System.Net.Sockets;

namespace Server;

public class Player
{
    public int Id;
    public string Nickname = "";
    public string Email = "";
    public TcpClient Client = null!;
    public NetworkStream Stream = null!;
    public ArchetypeType Archetype = ArchetypeType.Neutral;
    
    public Dictionary<Resources, int> ResourceStorage = new Dictionary<Resources, int>();
    public List<Building> Buildings = new List<Building>();
    public int Soldiers = 0;
    public int SoldiersCreatedThisTurn = 0;
    public HashSet<int> AttackedPlayersThisTurn = new HashSet<int>();

    public void InitResources()
    {
        ResourceStorage[Resources.Wood] = 10;
        ResourceStorage[Resources.Stone] = 10;
        ResourceStorage[Resources.Ore] = 5;
        ResourceStorage[Resources.Wheat] = 8;
    }

    public bool HasResource(Resources res, int amount)
    {
        if (!ResourceStorage.ContainsKey(res)) return false;
        return ResourceStorage[res] >= amount;
    }

    public void AddResource(Resources res, int amount)
    {
        if (!ResourceStorage.ContainsKey(res))
            ResourceStorage[res] = 0;
        ResourceStorage[res] += amount;
    }

    public void RemoveResource(Resources res, int amount)
    {
        if (ResourceStorage.ContainsKey(res))
            ResourceStorage[res] -= amount;
    }

    public int GetDefense()
    {
        int def = 0;
        foreach (var b in Buildings)
        {
            if (b.Type == BuildingType.Barricade)
            {
                if (b.Level == 1) def += 5;
                else if (b.Level == 2) def += 10;
            }
            else if (b.Type == BuildingType.DefenseTower)
            {
                if (b.Level == 1) def += 20;
                else if (b.Level == 2) def += 30;
            }
        }

        if (Archetype == ArchetypeType.Greedy)
            def = (int)(def * 1.3);
        else if (Archetype == ArchetypeType.Patron)
            def = (int)(def * 0.75);

        return def;
    }

    public int GetMaxSoldiersPerTurn()
    {
        int max = 0;
        foreach (var b in Buildings)
        {
            if (b.Type == BuildingType.Barracks)
            {
                max += GameLogic.GetProduction(BuildingType.Barracks, b.Level);
            }
        }
        return max;
    }

    public int CalcPoints()
    {
        int pts = 0;
        foreach (var r in ResourceStorage)
        {
            int baseP = GetResourcePoints(r.Key);
            int pointsPerUnit = baseP;

            if (Archetype == ArchetypeType.Greedy)
                pointsPerUnit = (int)(baseP * 0.8);
            else if (Archetype == ArchetypeType.Patron)
                pointsPerUnit = (int)(baseP * 1.25);
            else if (Archetype == ArchetypeType.Engineer)
                pointsPerUnit = (int)(baseP * 0.8);
            else if (Archetype == ArchetypeType.Alchemist)
            {
                if (r.Key == Resources.Gold || r.Key == Resources.Emerald)
                    pointsPerUnit = (int)(baseP * 1.25);
                else
                {
                    pointsPerUnit = (int)(baseP * 0.7);
                }
            }

            // Минимум 1 очко за единицу ресурса (важно для Алхимика)
            if (pointsPerUnit < 1) pointsPerUnit = 1;

            pts += pointsPerUnit * r.Value;
        }
        return pts;
    }

    private int GetResourcePoints(Resources res)
    {
        if (res == Resources.Wood || res == Resources.Stone || res == Resources.Ore || res == Resources.Wheat)
            return 1;
        if (res == Resources.Lumber || res == Resources.Bricks || res == Resources.Metal || res == Resources.Coal || res == Resources.Sand || res == Resources.Bread)
            return 3;
        if (res == Resources.Furniture || res == Resources.Walls || res == Resources.Tools || res == Resources.Glass || res == Resources.Weapon)
            return 8;
        if (res == Resources.Gold || res == Resources.Emerald)
            return 25;
        return 0;
    }
}
