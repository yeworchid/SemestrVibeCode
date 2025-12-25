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
    
    public Dictionary<string, int> Resources = new Dictionary<string, int>();
    public List<Building> Buildings = new List<Building>();
    public int Soldiers = 0;

    public void InitResources()
    {
        Resources["Wood"] = 10;
        Resources["Stone"] = 10;
        Resources["Ore"] = 5;
        Resources["Wheat"] = 8;
    }

    public bool HasResource(string name, int amount)
    {
        if (!Resources.ContainsKey(name)) return false;
        return Resources[name] >= amount;
    }

    public void AddResource(string name, int amount)
    {
        if (!Resources.ContainsKey(name))
            Resources[name] = 0;
        Resources[name] += amount;
    }

    public void RemoveResource(string name, int amount)
    {
        if (Resources.ContainsKey(name))
            Resources[name] -= amount;
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

    public int CalcPoints()
    {
        int pts = 0;
        foreach (var r in Resources)
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
                if (r.Key == "Gold" || r.Key == "Emerald")
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

    private int GetResourcePoints(string name)
    {
        if (name == "Wood" || name == "Stone" || name == "Ore" || name == "Wheat")
            return 1;
        if (name == "Lumber" || name == "Bricks" || name == "Metal" || name == "Coal" || name == "Sand" || name == "Bread")
            return 3;
        if (name == "Furniture" || name == "Walls" || name == "Tools" || name == "Glass" || name == "Weapon")
            return 8;
        if (name == "Gold" || name == "Emerald")
            return 25;
        return 0;
    }
}
