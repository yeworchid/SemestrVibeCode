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
        private Label lblWaiting;

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
            txtNickname = new TextBox();
            txtEmail = new TextBox();
            btnConnect = new Button();
            cmbArchetype = new ComboBox();
            btnSelectArchetype = new Button();
            pnlGame = new Panel();
            lblCycle = new Label();
            lblTurn = new Label();
            gameField = new GameFieldControl();
            lblResources = new Label();
            lstBuildings = new ListBox();
            btnBuild = new Button();
            btnUpgrade = new Button();
            btnMakeSoldiers = new Button();
            btnAttack = new Button();
            btnEndTurn = new Button();
            txtLog = new TextBox();
            lblWaiting = new Label();
            pnlGame.SuspendLayout();
            SuspendLayout();
            // 
            // txtNickname
            // 
            txtNickname.Location = new Point(20, 20);
            txtNickname.Name = "txtNickname";
            txtNickname.PlaceholderText = "Никнейм";
            txtNickname.Size = new Size(200, 35);
            txtNickname.TabIndex = 0;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(240, 20);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email";
            txtEmail.Size = new Size(200, 35);
            txtEmail.TabIndex = 1;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(460, 20);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(200, 35);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Подключиться";
            btnConnect.Click += BtnConnect_Click;
            // 
            // cmbArchetype
            // 
            cmbArchetype.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbArchetype.Location = new Point(960, 20);
            cmbArchetype.Name = "cmbArchetype";
            cmbArchetype.Size = new Size(200, 38);
            cmbArchetype.TabIndex = 3;
            cmbArchetype.Visible = false;
            // 
            // btnSelectArchetype
            // 
            btnSelectArchetype.Location = new Point(1180, 20);
            btnSelectArchetype.Name = "btnSelectArchetype";
            btnSelectArchetype.Size = new Size(220, 35);
            btnSelectArchetype.TabIndex = 4;
            btnSelectArchetype.Text = "Выбрать архетип";
            btnSelectArchetype.Visible = false;
            btnSelectArchetype.Click += BtnSelectArchetype_Click;
            // 
            // pnlGame
            // 
            pnlGame.BorderStyle = BorderStyle.FixedSingle;
            pnlGame.Controls.Add(lblWaiting);
            pnlGame.Controls.Add(lblCycle);
            pnlGame.Controls.Add(lblTurn);
            pnlGame.Controls.Add(gameField);
            pnlGame.Controls.Add(lblResources);
            pnlGame.Controls.Add(lstBuildings);
            pnlGame.Controls.Add(btnBuild);
            pnlGame.Controls.Add(btnUpgrade);
            pnlGame.Controls.Add(btnMakeSoldiers);
            pnlGame.Controls.Add(btnAttack);
            pnlGame.Controls.Add(btnEndTurn);
            pnlGame.Controls.Add(txtLog);
            pnlGame.Location = new Point(20, 80);
            pnlGame.Name = "pnlGame";
            pnlGame.Size = new Size(1380, 748);
            pnlGame.TabIndex = 5;
            pnlGame.Visible = false;
            // 
            // lblCycle
            // 
            lblCycle.Location = new Point(10, 27);
            lblCycle.Name = "lblCycle";
            lblCycle.Size = new Size(166, 32);
            lblCycle.TabIndex = 0;
            lblCycle.Text = "Цикл: 0";
            // 
            // lblTurn
            // 
            lblTurn.Location = new Point(10, 59);
            lblTurn.Name = "lblTurn";
            lblTurn.Size = new Size(166, 39);
            lblTurn.TabIndex = 1;
            lblTurn.Text = "Ход: 0";
            // 
            // gameField
            // 
            gameField.Location = new Point(10, 101);
            gameField.Name = "gameField";
            gameField.Size = new Size(400, 400);
            gameField.TabIndex = 2;
            gameField.PlaceClicked += GameField_PlaceClicked;
            // 
            // lblResources
            // 
            lblResources.Location = new Point(439, 10);
            lblResources.Name = "lblResources";
            lblResources.Size = new Size(297, 710);
            lblResources.TabIndex = 3;
            lblResources.Text = "Ресурсы:";
            // 
            // lstBuildings
            // 
            lstBuildings.Location = new Point(10, 506);
            lstBuildings.Name = "lstBuildings";
            lstBuildings.Size = new Size(409, 214);
            lstBuildings.TabIndex = 4;
            // 
            // btnBuild
            // 
            btnBuild.Location = new Point(1087, 32);
            btnBuild.Name = "btnBuild";
            btnBuild.Size = new Size(130, 30);
            btnBuild.TabIndex = 5;
            btnBuild.Text = "Построить";
            btnBuild.Click += BtnBuild_Click;
            // 
            // btnUpgrade
            // 
            btnUpgrade.Location = new Point(1087, 72);
            btnUpgrade.Name = "btnUpgrade";
            btnUpgrade.Size = new Size(130, 30);
            btnUpgrade.TabIndex = 6;
            btnUpgrade.Text = "Улучшить";
            btnUpgrade.Click += BtnUpgrade_Click;
            // 
            // btnMakeSoldiers
            // 
            btnMakeSoldiers.Location = new Point(1087, 112);
            btnMakeSoldiers.Name = "btnMakeSoldiers";
            btnMakeSoldiers.Size = new Size(130, 30);
            btnMakeSoldiers.TabIndex = 7;
            btnMakeSoldiers.Text = "Создать солдат";
            btnMakeSoldiers.Click += BtnMakeSoldiers_Click;
            // 
            // btnAttack
            // 
            btnAttack.Location = new Point(1087, 152);
            btnAttack.Name = "btnAttack";
            btnAttack.Size = new Size(130, 30);
            btnAttack.TabIndex = 8;
            btnAttack.Text = "Атаковать";
            btnAttack.Click += BtnAttack_Click;
            // 
            // btnEndTurn
            // 
            btnEndTurn.Enabled = false;
            btnEndTurn.Location = new Point(1087, 347);
            btnEndTurn.Name = "btnEndTurn";
            btnEndTurn.Size = new Size(130, 40);
            btnEndTurn.TabIndex = 9;
            btnEndTurn.Text = "Завершить ход";
            btnEndTurn.Click += BtnEndTurn_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(1223, 27);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(140, 355);
            txtLog.TabIndex = 10;
            // 
            // lblWaiting
            // 
            lblWaiting.AutoSize = true;
            lblWaiting.ForeColor = Color.Red;
            lblWaiting.Location = new Point(1087, 200);
            lblWaiting.Name = "lblWaiting";
            lblWaiting.Size = new Size(130, 30);
            lblWaiting.TabIndex = 11;
            lblWaiting.Text = "";
            lblWaiting.Visible = false;
            // 
            // Form1
            // 
            ClientSize = new Size(1416, 836);
            Controls.Add(txtNickname);
            Controls.Add(txtEmail);
            Controls.Add(btnConnect);
            Controls.Add(cmbArchetype);
            Controls.Add(btnSelectArchetype);
            Controls.Add(pnlGame);
            MaximumSize = new Size(1440, 900);
            MinimumSize = new Size(1440, 900);
            Name = "Form1";
            Text = "Игра - Клиент";
            pnlGame.ResumeLayout(false);
            pnlGame.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
