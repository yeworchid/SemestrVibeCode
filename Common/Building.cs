using System;
using System.Collections.Generic;
using System.Text;

namespace Common;

public class Building
{
    public BuildingType Type { get; set; }
    public int Level { get; set; }
    public int PlaceId { get; set; }
    public int TurnBuilt { get; set; } // Для проверки возможности улучшения
    
    public Building(BuildingType type, int placeId, int turnBuilt)
    {
        Type = type;
        Level = 1;
        PlaceId = placeId;
        TurnBuilt = turnBuilt;
    }
}
