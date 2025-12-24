using Common;
using System.Net.Sockets;

namespace Server;

public class Player
{
    public int Id { get; set; }
    public string Nickname { get; set; } = "";
    public string Email { get; set; } = "";
    public TcpClient Client { get; set; } = null!;
    public ArchetypeType Archetype { get; set; }
    public Dictionary<Resources, int> Resources { get; set; }
    public List<Building> Buildings { get; set; }
    public int Soldiers { get; set; }
    public int TotalDefense { get; set; }

    public Player()
    {
        Resources = new Dictionary<Resources, int>();
        Buildings = new List<Building>();
        Soldiers = 0;
        TotalDefense = 0;
    }

    public void AddResource(Resources res, int amount)
    {
        if (!Resources.ContainsKey(res))
            Resources[res] = 0;
        Resources[res] += amount;
    }

    public bool HasResources(Dictionary<Resources, int> cost)
    {
        foreach (var item in cost)
        {
            if (!Resources.ContainsKey(item.Key) || Resources[item.Key] < item.Value)
                return false;
        }
        return true;
    }

    public void SpendResources(Dictionary<Resources, int> cost)
    {
        foreach (var item in cost)
        {
            Resources[item.Key] -= item.Value;
        }
    }

    public int CalculatePoints()
    {
        int points = 0;
        foreach (var res in Resources)
        {
            if (res.Key == Common.Resources.Soldier)
                continue;
            int basePoints = GameLogic.GetResourcePoints(res.Key);
            int finalPoints = basePoints;

            if (Archetype == ArchetypeType.Greedy)
            {
                finalPoints = (int)(basePoints * 0.8);
            }
            else if (Archetype == ArchetypeType.Patron)
            {
                finalPoints = (int)(basePoints * 1.25);
            }
            else if (Archetype == ArchetypeType.Engineer)
            {
                finalPoints = (int)(basePoints * 0.8);
            }
            else if (Archetype == ArchetypeType.Alchemist)
            {
                if (res.Key == Common.Resources.Gold || res.Key == Common.Resources.Emerald)
                {
                    finalPoints = (int)(basePoints * 1.25);
                }
                else
                {
                    finalPoints = (int)(basePoints * 0.7);
                    if (finalPoints < 1) finalPoints = 1;
                }
            }

            points += finalPoints * res.Value;
        }
        return points;
    }

    public void RecalculateDefense()
    {
        int defense = 0;
        foreach (var b in Buildings)
        {
            defense += GameLogic.GetDefenseBonus(b.Type, b.Level);
        }
        if (Archetype == ArchetypeType.Greedy)
        {
            defense = (int)(defense * 1.3);
        }
        else if (Archetype == ArchetypeType.Patron)
        {
            defense = (int)(defense * 0.75);
        }
        TotalDefense = defense;
    }
}
