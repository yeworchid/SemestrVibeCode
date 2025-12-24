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
            this.txtNickname.PlaceholderText = "Nickname";

            this.txtEmail.Location = new Point(20, 50);
            this.txtEmail.Size = new Size(200, 25);
            this.txtEmail.PlaceholderText = "Email";

            this.btnConnect.Location = new Point(20, 80);
            this.btnConnect.Size = new Size(200, 30);
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += BtnConnect_Click;

            this.cmbArchetype.Location = new Point(20, 120);
            this.cmbArchetype.Size = new Size(200, 25);
            this.cmbArchetype.Visible = false;

            this.btnSelectArchetype.Location = new Point(20, 150);
            this.btnSelectArchetype.Size = new Size(200, 30);
            this.btnSelectArchetype.Text = "Select Archetype";
            this.btnSelectArchetype.Visible = false;
            this.btnSelectArchetype.Click += BtnSelectArchetype_Click;

            this.pnlGame.Location = new Point(20, 200);
            this.pnlGame.Size = new Size(760, 450);
            this.pnlGame.Visible = false;

            this.lblCycle.Location = new Point(10, 10);
            this.lblCycle.Size = new Size(200, 20);

            this.lblTurn.Location = new Point(10, 35);
            this.lblTurn.Size = new Size(200, 20);

            this.lblResources.Location = new Point(280, 10);
            this.lblResources.Size = new Size(200, 250);

            this.gameField.Location = new Point(10, 60);
            this.gameField.Size = new Size(260, 260);

            this.lstBuildings.Location = new Point(10, 330);
            this.lstBuildings.Size = new Size(260, 100);

            this.btnBuild.Location = new Point(490, 60);
            this.btnBuild.Size = new Size(120, 30);
            this.btnBuild.Text = "Build";
            this.btnBuild.Click += BtnBuild_Click;

            this.btnUpgrade.Location = new Point(490, 100);
            this.btnUpgrade.Size = new Size(120, 30);
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.Click += BtnUpgrade_Click;

            this.btnMakeSoldiers.Location = new Point(490, 140);
            this.btnMakeSoldiers.Size = new Size(120, 30);
            this.btnMakeSoldiers.Text = "Make Soldiers";
            this.btnMakeSoldiers.Click += BtnMakeSoldiers_Click;

            this.btnAttack.Location = new Point(490, 180);
            this.btnAttack.Size = new Size(120, 30);
            this.btnAttack.Text = "Attack";
            this.btnAttack.Click += BtnAttack_Click;

            this.btnEndTurn.Location = new Point(490, 380);
            this.btnEndTurn.Size = new Size(120, 40);
            this.btnEndTurn.Text = "End Turn";
            this.btnEndTurn.Click += BtnEndTurn_Click;

            this.txtLog.Location = new Point(620, 60);
            this.txtLog.Size = new Size(130, 360);
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;

            this.pnlGame.Controls.Add(this.lblCycle);
            this.pnlGame.Controls.Add(this.lblTurn);
            this.pnlGame.Controls.Add(this.lblResources);
            this.pnlGame.Controls.Add(this.gameField);
            this.pnlGame.Controls.Add(this.lstBuildings);
            this.pnlGame.Controls.Add(this.btnBuild);
            this.pnlGame.Controls.Add(this.btnUpgrade);
            this.pnlGame.Controls.Add(this.btnMakeSoldiers);
            this.pnlGame.Controls.Add(this.btnAttack);
            this.pnlGame.Controls.Add(this.btnEndTurn);
            this.pnlGame.Controls.Add(this.txtLog);

            this.ClientSize = new Size(800, 670);
            this.Controls.Add(this.txtNickname);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbArchetype);
            this.Controls.Add(this.btnSelectArchetype);
            this.Controls.Add(this.pnlGame);
            this.Text = "Game Client";

            this.ResumeLayout(false);
        }
    }
}
