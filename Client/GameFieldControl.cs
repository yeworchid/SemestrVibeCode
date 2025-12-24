using Common;
using Common.DTO;

namespace Client
{
    public class GameFieldControl : Control
    {
        private List<BuildingStateDto> buildings = new List<BuildingStateDto>();
        private int selectedPlace = -1;
        private const int CellSize = 50;
        private const int GridSize = 5;

        public event EventHandler<int> PlaceClicked;

        public GameFieldControl()
        {
            this.Size = new Size(GridSize * CellSize + 10, GridSize * CellSize + 10);
            this.DoubleBuffered = true;
            this.MouseClick += OnMouseClick;
        }

        public void UpdateBuildings(List<BuildingStateDto> newBuildings)
        {
            buildings = newBuildings;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int placeId = row * GridSize + col;
                    int x = col * CellSize + 5;
                    int y = row * CellSize + 5;

                    var building = buildings.FirstOrDefault(b => b.PlaceId == placeId);
                    
                    Brush brush = Brushes.LightGray;
                    if (building != null)
                    {
                        if (building.Type == BuildingType.Barracks)
                            brush = Brushes.Red;
                        else if (building.Type == BuildingType.Barricade || building.Type == BuildingType.DefenseTower)
                            brush = Brushes.Blue;
                        else if (building.Type == BuildingType.Laboratory || building.Type == BuildingType.AlchemyFurnace)
                            brush = Brushes.Gold;
                        else
                            brush = Brushes.Green;
                    }

                    if (placeId == selectedPlace)
                    {
                        g.FillRectangle(Brushes.Yellow, x, y, CellSize - 2, CellSize - 2);
                    }
                    else
                    {
                        g.FillRectangle(brush, x, y, CellSize - 2, CellSize - 2);
                    }

                    g.DrawRectangle(Pens.Black, x, y, CellSize - 2, CellSize - 2);

                    if (building != null)
                    {
                        string text = building.Level.ToString();
                        var font = new Font("Arial", 16, FontStyle.Bold);
                        var size = g.MeasureString(text, font);
                        g.DrawString(text, font, Brushes.White, 
                            x + (CellSize - size.Width) / 2, 
                            y + (CellSize - size.Height) / 2);
                    }
                }
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            int col = (e.X - 5) / CellSize;
            int row = (e.Y - 5) / CellSize;

            if (col >= 0 && col < GridSize && row >= 0 && row < GridSize)
            {
                int placeId = row * GridSize + col;
                selectedPlace = placeId;
                this.Invalidate();
                PlaceClicked?.Invoke(this, placeId);
            }
        }
    }
}
