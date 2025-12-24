using Common;

namespace Server;

public class Player
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public ArchetypeType Archetype { get; set; }
    public Dictionary<string, int> Resources { get; set; }
    public int Soldiers { get; set; }
    public Building?[] Field { get; set; } // 25 клеток (5x5)
    public int CurrentTurn { get; set; }
    
    public Player(int id, string nickname, string email)
    {
        Id = id;
        Nickname = nickname;
        Email = email;
        Archetype = ArchetypeType.Neutral;
        Resources = new Dictionary<string, int>(GameConfig.StartingResources);
        Soldiers = 0;
        Field = new Building?[GameConfig.FieldSize * GameConfig.FieldSize];
        CurrentTurn = 0;
    }
    
    public int GetDefense()
    {
        int defense = 0;
        foreach (var building in Field)
        {
            if (building != null && BuildingConfig.Buildings.TryGetValue(building.Type, out var info))
            {
                if (info.Category == BuildingCategory.Defense)
                {
                    var levelInfo = info.Levels[building.Level];
                    defense += levelInfo.DefenseBonus ?? 0;
                }
            }
        }
        
        // Применяем модификатор архетипа
        if (Archetype == ArchetypeType.Greedy)
            defense = (int)(defense * 1.3);
        else if (Archetype == ArchetypeType.Patron)
            defense = (int)(defense * 0.75);
        
        return defense;
    }
    
    public int CalculatePoints()
    {
        int points = 0;
        
        foreach (var (resource, count) in Resources)
        {
            if (GameConfig.ResourcePoints.TryGetValue(resource, out int basePoints))
            {
                int resourcePoints = basePoints * count;
                
                // Применяем модификаторы архетипа
                if (Archetype == ArchetypeType.Greedy || Archetype == ArchetypeType.Engineer)
                {
                    resourcePoints = (int)(resourcePoints * 0.8);
                }
                else if (Archetype == ArchetypeType.Patron)
                {
                    resourcePoints = (int)(resourcePoints * 1.25);
                }
                else if (Archetype == ArchetypeType.Alchemist)
                {
                    if (resource == "Gold" || resource == "Emerald")
                        resourcePoints = (int)(resourcePoints * 1.25);
                    else
                        resourcePoints = Math.Max(count, (int)(resourcePoints * 0.7));
                }
                
                points += resourcePoints;
            }
        }
        
        return points;
    }
    
    public bool HasResources(Dictionary<string, int> cost)
    {
        foreach (var (resource, amount) in cost)
        {
            if (!Resources.TryGetValue(resource, out int available) || available < amount)
                return false;
        }
        return true;
    }
    
    public void ConsumeResources(Dictionary<string, int> cost)
    {
        foreach (var (resource, amount) in cost)
        {
            Resources[resource] -= amount;
            if (Resources[resource] <= 0)
                Resources.Remove(resource);
        }
    }
    
    public void AddResources(Dictionary<string, int> resources)
    {
        foreach (var (resource, amount) in resources)
        {
            if (!Resources.ContainsKey(resource))
                Resources[resource] = 0;
            Resources[resource] += amount;
        }
    }
}
