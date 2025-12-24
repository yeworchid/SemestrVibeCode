using Common;

namespace Client
{
    public class BuildForm : Form
    {
        private ComboBox cmbType;
        private Button btnOk;
        private Button btnCancel;

        public BuildingType SelectedType { get; private set; }

        public BuildForm()
        {
            InitializeComponent();
            LoadTypes();
        }

        private void InitializeComponent()
        {
            this.cmbType = new ComboBox();
            this.btnOk = new Button();
            this.btnCancel = new Button();

            this.cmbType.Location = new Point(20, 20);
            this.cmbType.Size = new Size(250, 25);
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;

            this.btnOk.Location = new Point(20, 60);
            this.btnOk.Size = new Size(100, 30);
            this.btnOk.Text = "OK";
            this.btnOk.Click += BtnOk_Click;

            this.btnCancel.Location = new Point(170, 60);
            this.btnCancel.Size = new Size(100, 30);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(300, 110);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Text = "Построить здание";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadTypes()
        {
            cmbType.Items.Add("Лесозаготовка");
            cmbType.Items.Add("Карьер");
            cmbType.Items.Add("Шахта");
            cmbType.Items.Add("Поле");
            cmbType.Items.Add("Лесопилка");
            cmbType.Items.Add("Печь обжига");
            cmbType.Items.Add("Плавильня");
            cmbType.Items.Add("Углежог");
            cmbType.Items.Add("Дробилка");
            cmbType.Items.Add("Пекарня");
            cmbType.Items.Add("Столярная");
            cmbType.Items.Add("Каменотёс");
            cmbType.Items.Add("Кузница");
            cmbType.Items.Add("Стекловарня");
            cmbType.Items.Add("Оружейная");
            cmbType.Items.Add("Казармы");
            cmbType.Items.Add("Лаборатория");
            cmbType.Items.Add("Алхимическая печь");
            cmbType.Items.Add("Баррикада");
            cmbType.Items.Add("Оборонительная башня");
            cmbType.SelectedIndex = 0;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            SelectedType = (BuildingType)cmbType.SelectedIndex;
            this.DialogResult = DialogResult.OK;
        }
    }
}
