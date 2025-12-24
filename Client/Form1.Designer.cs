namespace Client
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtNickname;
        private TextBox txtEmail;
        private Button btnConnect;
        private ComboBox cmbArchetype;
        private Button btnSelectArchetype;
        private Panel pnlGame;
        private Label lblCycle;
        private Label lblTurn;
        private Label lblResources;
        private Button btnEndTurn;
        private ListBox lstBuildings;
        private Button btnBuild;
        private Button btnUpgrade;
        private Button btnMakeSoldiers;
        private Button btnAttack;
        private TextBox txtLog;
        private GameFieldControl gameField;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtNickname = new TextBox();
            this.txtEmail = new TextBox();
            this.btnConnect = new Button();
            this.cmbArchetype = new ComboBox();
            this.btnSelectArchetype = new Button();
            this.pnlGame = new Panel();
            this.lblCycle = new Label();
            this.lblTurn = new Label();
            this.lblResources = new Label();
            this.btnEndTurn = new Button();
            this.lstBuildings = new ListBox();
            this.btnBuild = new Button();
            this.btnUpgrade = new Button();
            this.btnMakeSoldiers = new Button();
            this.btnAttack = new Button();
            this.txtLog = new TextBox();
            this.gameField = new GameFieldControl();

            this.SuspendLayout();

            this.txtNickname.Location = new Point(20, 20);
            this.txtNickname.Size = new Size(200, 25);
            this.txtNickname.PlaceholderText = "Никнейм";

            this.txtEmail.Location = new Point(20, 50);
            this.txtEmail.Size = new Size(200, 25);
            this.txtEmail.PlaceholderText = "Email";

            this.btnConnect.Location = new Point(20, 80);
            this.btnConnect.Size = new Size(200, 30);
            this.btnConnect.Text = "Подключиться";
            this.btnConnect.Click += BtnConnect_Click;

            this.cmbArchetype.Location = new Point(20, 120);
            this.cmbArchetype.Size = new Size(200, 25);
            this.cmbArchetype.Visible = false;
            this.cmbArchetype.DropDownStyle = ComboBoxStyle.DropDownList;

            this.btnSelectArchetype.Location = new Point(20, 150);
            this.btnSelectArchetype.Size = new Size(200, 30);
            this.btnSelectArchetype.Text = "Выбрать архетип";
            this.btnSelectArchetype.Visible = false;
            this.btnSelectArchetype.Click += BtnSelectArchetype_Click;

            this.pnlGame.Location = new Point(20, 200);
            this.pnlGame.Size = new Size(760, 420);
            this.pnlGame.Visible = false;
            this.pnlGame.BorderStyle = BorderStyle.FixedSingle;

            this.lblCycle.Location = new Point(10, 10);
            this.lblCycle.Size = new Size(150, 20);
            this.lblCycle.Text = "Цикл: 0";

            this.lblTurn.Location = new Point(10, 30);
            this.lblTurn.Size = new Size(150, 20);
            this.lblTurn.Text = "Ход: 0";

            this.gameField.Location = new Point(10, 55);
            this.gameField.PlaceClicked += GameField_PlaceClicked;

            this.lblResources.Location = new Point(280, 10);
            this.lblResources.Size = new Size(180, 250);
            this.lblResources.Text = "Ресурсы:";

            this.lstBuildings.Location = new Point(10, 320);
            this.lstBuildings.Size = new Size(260, 90);

            this.btnBuild.Location = new Point(470, 55);
            this.btnBuild.Size = new Size(130, 30);
            this.btnBuild.Text = "Построить";
            this.btnBuild.Click += BtnBuild_Click;

            this.btnUpgrade.Location = new Point(470, 95);
            this.btnUpgrade.Size = new Size(130, 30);
            this.btnUpgrade.Text = "Улучшить";
            this.btnUpgrade.Click += BtnUpgrade_Click;

            this.btnMakeSoldiers.Location = new Point(470, 135);
            this.btnMakeSoldiers.Size = new Size(130, 30);
            this.btnMakeSoldiers.Text = "Создать солдат";
            this.btnMakeSoldiers.Click += BtnMakeSoldiers_Click;

            this.btnAttack.Location = new Point(470, 175);
            this.btnAttack.Size = new Size(130, 30);
            this.btnAttack.Text = "Атаковать";
            this.btnAttack.Click += BtnAttack_Click;

            this.btnEndTurn.Location = new Point(470, 370);
            this.btnEndTurn.Size = new Size(130, 40);
            this.btnEndTurn.Text = "Завершить ход";
            this.btnEndTurn.Enabled = false;
            this.btnEndTurn.Click += BtnEndTurn_Click;

            this.txtLog.Location = new Point(610, 55);
            this.txtLog.Size = new Size(140, 355);
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;

            this.pnlGame.Controls.Add(this.lblCycle);
            this.pnlGame.Controls.Add(this.lblTurn);
            this.pnlGame.Controls.Add(this.gameField);
            this.pnlGame.Controls.Add(this.lblResources);
            this.pnlGame.Controls.Add(this.lstBuildings);
            this.pnlGame.Controls.Add(this.btnBuild);
            this.pnlGame.Controls.Add(this.btnUpgrade);
            this.pnlGame.Controls.Add(this.btnMakeSoldiers);
            this.pnlGame.Controls.Add(this.btnAttack);
            this.pnlGame.Controls.Add(this.btnEndTurn);
            this.pnlGame.Controls.Add(this.txtLog);

            this.ClientSize = new Size(800, 640);
            this.Controls.Add(this.txtNickname);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbArchetype);
            this.Controls.Add(this.btnSelectArchetype);
            this.Controls.Add(this.pnlGame);
            this.Text = "Игра - Клиент";

            this.ResumeLayout(false);
        }
    }
}
