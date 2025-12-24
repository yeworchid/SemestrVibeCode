using Common;
using Common.DTO;

namespace Client;

public class GameFieldControl : Panel
{
    private const int CellSize = 80;
    private const int GridSize = 5;
    private Button[,] _cells;
    private StateDto? _state;
    
    public event Action<int>? CellClicked;
    
    public GameFieldControl()
    {
        Width = CellSize * GridSize + 10;
        Height = CellSize * GridSize + 10;
        BorderStyle = BorderStyle.FixedSingle;
        
        _cells = new Button[GridSize, GridSize];
        
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                int placeId = y * GridSize + x;
                var btn = new Button
                {
                    Location = new Point(x * CellSize + 5, y * CellSize + 5),
                    Size = new Size(CellSize - 2, CellSize - 2),
                    Tag = placeId,
                    BackColor = Color.LightGray,
                    Font = new Font("Arial", 8)
                };
                
                btn.Click += (s, e) => CellClicked?.Invoke(placeId);
                
                _cells[y, x] = btn;
                Controls.Add(btn);
            }
        }
    }
    
    public void UpdateState(StateDto state)
    {
        _state = state;
        
        // Очищаем все клетки
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                _cells[y, x].Text = "";
                _cells[y, x].BackColor = Color.LightGray;
            }
        }
        
        // Отображаем здания
        foreach (var building in state.Buildings)
        {
            int y = building.PlaceId / GridSize;
            int x = building.PlaceId % GridSize;
            
            var btn = _cells[y, x];
            btn.Text = GetBuildingShortName(building.Type) + $"\nLv{building.Level}";
            btn.BackColor = GetBuildingColor(building.Type);
        }
    }
    
    private string GetBuildingShortName(BuildingType type)
    {
        return type switch
        {
            BuildingType.Logging => "Лес",
            BuildingType.Quarry => "Карьер",
            BuildingType.Mine => "Шахта",
            BuildingType.Farm => "Поле",
            BuildingType.Sawmill => "Пилка",
            BuildingType.KilnFurnace => "Печь",
            BuildingType.Smelter => "Плавка",
            BuildingType.Charcoal => "Уголь",
            BuildingType.Crusher => "Дробка",
            BuildingType.Bakery => "Пекарня",
            BuildingType.Carpentry => "Столяр",
            BuildingType.Stonemason => "Камень",
            BuildingType.Forge => "Кузня",
            BuildingType.Glassworks => "Стекло",
            BuildingType.Armory => "Оружие",
            BuildingType.Laboratory => "Лаб",
            BuildingType.AlchemicalFurnace => "Алхим",
            BuildingType.Barracks => "Казарма",
            BuildingType.Barricade => "Баррик",
            BuildingType.DefenseTower => "Башня",
            _ => "???"
        };
    }
    
    private Color GetBuildingColor(BuildingType type)
    {
        return type switch
        {
            BuildingType.Logging or BuildingType.Quarry or BuildingType.Mine or BuildingType.Farm => Color.LightGreen,
            BuildingType.Sawmill or BuildingType.KilnFurnace or BuildingType.Smelter or BuildingType.Charcoal or BuildingType.Crusher or BuildingType.Bakery => Color.LightBlue,
            BuildingType.Carpentry or BuildingType.Stonemason or BuildingType.Forge or BuildingType.Glassworks or BuildingType.Armory => Color.LightCoral,
            BuildingType.Laboratory or BuildingType.AlchemicalFurnace => Color.Gold,
            BuildingType.Barracks => Color.Orange,
            BuildingType.Barricade or BuildingType.DefenseTower => Color.Brown,
            _ => Color.Gray
        };
    }
}
