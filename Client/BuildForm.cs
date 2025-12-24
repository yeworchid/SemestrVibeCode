using Common;

namespace Client
{
    public class BuildForm : Form
    {
        private ComboBox cmbBuildingType;
        private TextBox txtPlaceId;
        private Button btnOk;
        private Button btnCancel;

        public int PlaceId { get; private set; }
        public BuildingType BuildingType { get; private set; }

        public BuildForm()
        {
            InitializeComponent();
            LoadBuildingTypes();
        }

        private void InitializeComponent()
        {
            this.cmbBuildingType = new ComboBox();
            this.txtPlaceId = new TextBox();
            this.btnOk = new Button();
            this.btnCancel = new Button();

            this.cmbBuildingType.Location = new Point(20, 20);
            this.cmbBuildingType.Size = new Size(250, 25);
            this.cmbBuildingType.DropDownStyle = ComboBoxStyle.DropDownList;

            this.txtPlaceId.Location = new Point(20, 60);
            this.txtPlaceId.Size = new Size(250, 25);
            this.txtPlaceId.PlaceholderText = "Place ID (0-24)";

            this.btnOk.Location = new Point(20, 100);
            this.btnOk.Size = new Size(100, 30);
            this.btnOk.Text = "OK";
            this.btnOk.Click += BtnOk_Click;

            this.btnCancel.Location = new Point(170, 100);
            this.btnCancel.Size = new Size(100, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(300, 150);
            this.Controls.Add(this.cmbBuildingType);
            this.Controls.Add(this.txtPlaceId);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Text = "Build";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadBuildingTypes()
        {
            foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
            {
                cmbBuildingType.Items.Add(type.ToString());
            }
            cmbBuildingType.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPlaceId.Text, out int placeId))
            {
                PlaceId = placeId;
                BuildingType = (BuildingType)cmbBuildingType.SelectedIndex;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid place ID");
            }
        }
    }
}
