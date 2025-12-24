using System.Text.Json;
using Common;
using Common.DTO;

namespace Client
{
    public partial class Form1 : Form
    {
        private GameClient _client;
        private int _myPlayerId;
        private ArchetypeType _offeredArchetype;
        private StateDto? _currentState;
        
        private TextBox txtNickname;
        private TextBox txtEmail;
        private TextBox txtServer;
        private Button btnConnect;
        private RichTextBox txtLog;
        private Panel pnlGame;
        private Button btnAcceptArchetype;
        private Button btnDeclineArchetype;
        private Label lblArchetype;
        private Button btnEndTurn;
        private GameFieldControl fieldControl;
        private ComboBox cmbBuildingType;
        private Button btnBuild;
        private Button btnUpgrade;
        private NumericUpDown numSoldiers;
        private Button btnMakeSoldiers;
        private ComboBox cmbAttackTarget;
        private NumericUpDown numAttackSoldiers;
        private Button btnAttack;
        private Label lblResources;
        private Label lblInfo;
        private int _selectedCell = -1;
        
        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            _client = new GameClient();
            _client.MessageReceived += OnMessageReceived;
        }
        
        private void InitializeCustomComponents()
        {
            this.Text = "ОРИС Игра - Клиент";
            this.Size = new Size(1200, 800);
            
            // Панель подключения
            var pnlConnect = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            txtNickname = new TextBox { Location = new Point(10, 10), Width = 150 };
            var lblNick = new Label { Text = "Никнейм:", Location = new Point(10, 35), AutoSize = true };
            
            txtEmail = new TextBox { Location = new Point(170, 10), Width = 150 };
            var lblEmail = new Label { Text = "Email:", Location = new Point(170, 35), AutoSize = true };
            
            txtServer = new TextBox { Location = new Point(330, 10), Width = 150, Text = "localhost:5000" };
            var lblServer = new Label { Text = "Сервер:", Location = new Point(330, 35), AutoSize = true };
            
            btnConnect = new Button { Text = "Подключиться", Location = new Point(490, 10), Width = 120, Height = 30 };
            btnConnect.Click += BtnConnect_Click;
            
            pnlConnect.Controls.AddRange(new Control[] { txtNickname, lblNick, txtEmail, lblEmail, txtServer, lblServer, btnConnect });
            
            // Лог
            txtLog = new RichTextBox
            {
                Location = new Point(10, 500),
                Size = new Size(1160, 250),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            // Панель игры
            pnlGame = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(1180, 410),
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            // Игровое поле
            fieldControl = new GameFieldControl
            {
                Location = new Point(10, 10)
            };
            fieldControl.CellClicked += OnCellClicked;
            
            // Информация
            lblInfo = new Label
            {
                Location = new Point(430, 10),
                Size = new Size(300, 100),
                Font = new Font("Arial", 10)
            };
            
            lblResources = new Label
            {
                Location = new Point(430, 120),
                Size = new Size(300, 200),
                Font = new Font("Courier New", 9)
            };
            
            // Архетип
            lblArchetype = new Label
            {
                Location = new Point(750, 10),
                Size = new Size(400, 60),
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            
            btnAcceptArchetype = new Button
            {
                Text = "Принять архетип",
                Location = new Point(750, 80),
                Size = new Size(150, 35),
                Visible = false
            };
            btnAcceptArchetype.Click += BtnAcceptArchetype_Click;
            
            btnDeclineArchetype = new Button
            {
                Text = "Отказаться",
                Location = new Point(910, 80),
                Size = new Size(150, 35),
                Visible = false
            };
            btnDeclineArchetype.Click += BtnDeclineArchetype_Click;
            
            // Строительство
            var lblBuild = new Label { Text = "Строительство:", Location = new Point(750, 130), AutoSize = true };
            cmbBuildingType = new ComboBox
            {
                Location = new Point(750, 150),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
            {
                cmbBuildingType.Items.Add(type);
            }
            cmbBuildingType.SelectedIndex = 0;
            
            btnBuild = new Button
            {
                Text = "Построить",
                Location = new Point(960, 150),
                Size = new Size(100, 25)
            };
            btnBuild.Click += BtnBuild_Click;
            
            btnUpgrade = new Button
            {
                Text = "Улучшить",
                Location = new Point(960, 180),
                Size = new Size(100, 25)
            };
            btnUpgrade.Click += BtnUpgrade_Click;
            
            // Солдаты
            var lblSoldiers = new Label { Text = "Производство солдат:", Location = new Point(750, 220), AutoSize = true };
            numSoldiers = new NumericUpDown
            {
                Location = new Point(750, 240),
                Width = 100,
                Minimum = 1,
                Maximum = 10
            };
            btnMakeSoldiers = new Button
            {
                Text = "Произвести",
                Location = new Point(860, 240),
                Size = new Size(100, 25)
            };
            btnMakeSoldiers.Click += BtnMakeSoldiers_Click;
            
            // Атака
            var lblAttack = new Label { Text = "Атака:", Location = new Point(750, 280), AutoSize = true };
            cmbAttackTarget = new ComboBox
            {
                Location = new Point(750, 300),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            numAttackSoldiers = new NumericUpDown
            {
                Location = new Point(910, 300),
                Width = 80,
                Minimum = 1,
                Maximum = 100
            };
            btnAttack = new Button
            {
                Text = "Атаковать",
                Location = new Point(1000, 300),
                Size = new Size(100, 25)
            };
            btnAttack.Click += BtnAttack_Click;
            
            // Завершить ход
            btnEndTurn = new Button
            {
                Text = "ЗАВЕРШИТЬ ХОД",
                Location = new Point(750, 350),
                Size = new Size(350, 50),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Visible = false
            };
            btnEndTurn.Click += BtnEndTurn_Click;
            
            pnlGame.Controls.AddRange(new Control[] {
                fieldControl, lblInfo, lblResources, lblArchetype,
                btnAcceptArchetype, btnDeclineArchetype,
                lblBuild, cmbBuildingType, btnBuild, btnUpgrade,
                lblSoldiers, numSoldiers, btnMakeSoldiers,
                lblAttack, cmbAttackTarget, numAttackSoldiers, btnAttack,
                btnEndTurn
            });
            
            this.Controls.Add(pnlGame);
            this.Controls.Add(pnlConnect);
            this.Controls.Add(txtLog);
        }
        
        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Введите никнейм и email");
                return;
            }
            
            var parts = txtServer.Text.Split(':');
            var host = parts[0];
            var port = parts.Length > 1 ? int.Parse(parts[1]) : 5000;
            
            Log("Подключение к серверу...");
            
            if (await _client.Connect(host, port))
            {
                Log("Подключено! Отправка данных...");
                
                await _client.SendMessage(MessageType.JOIN, new JoinDto
                {
                    Nickname = txtNickname.Text,
                    Email = txtEmail.Text
                });
                
                btnConnect.Enabled = false;
            }
            else
            {
                Log("Ошибка подключения!");
            }
        }
        
        private void OnMessageReceived(NetworkMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnMessageReceived(message));
                return;
            }
            
            switch (message.Type)
            {
                case MessageType.START_GAME:
                    HandleStartGame(message);
                    break;
                
                case MessageType.START_TURN:
                    HandleStartTurn(message);
                    break;
                
                case MessageType.PRODUCTION_RESULT:
                    HandleProductionResult(message);
                    break;
                
                case MessageType.STATE:
                    HandleState(message);
                    break;
                
                case MessageType.ATTACK_TARGET:
                    HandleAttackTarget(message);
                    break;
                
                case MessageType.TURN_ENDED:
                    HandleTurnEnded(message);
                    break;
                
                case MessageType.GAME_END:
                    HandleGameEnd(message);
                    break;
                
                case MessageType.RESPONSE:
                    HandleResponse(message);
                    break;
            }
        }
        
        private void HandleStartGame(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<StartGameDto>(message.Payload);
            if (dto == null) return;
            
            _myPlayerId = dto.PlayerId;
            _offeredArchetype = dto.ArchetypeType;
            
            Log($"Игра начинается! Игроков: {dto.Players}, Циклов: {dto.Cycles}");
            Log($"Вам предложен архетип: {GetArchetypeName(dto.ArchetypeType)}");
            
            lblArchetype.Text = $"Предложенный архетип: {GetArchetypeName(dto.ArchetypeType)}\n{GetArchetypeDescription(dto.ArchetypeType)}";
            btnAcceptArchetype.Visible = true;
            btnDeclineArchetype.Visible = true;
            pnlGame.Visible = true;
            
            // Заполняем список целей для атаки
            cmbAttackTarget.Items.Clear();
            for (int i = 0; i < dto.Players; i++)
            {
                if (i != _myPlayerId)
                {
                    cmbAttackTarget.Items.Add(i);
                }
            }
            if (cmbAttackTarget.Items.Count > 0)
                cmbAttackTarget.SelectedIndex = 0;
        }
        
        private async void BtnAcceptArchetype_Click(object? sender, EventArgs e)
        {
            await _client.SendMessage(MessageType.ARCHETYPE, new ArchetypeDto
            {
                SelectedArchetype = _offeredArchetype
            });
            
            Log($"Вы приняли архетип: {GetArchetypeName(_offeredArchetype)}");
            btnAcceptArchetype.Visible = false;
            btnDeclineArchetype.Visible = false;
        }
        
        private async void BtnDeclineArchetype_Click(object? sender, EventArgs e)
        {
            await _client.SendMessage(MessageType.ARCHETYPE, new ArchetypeDto
            {
                SelectedArchetype = ArchetypeType.Neutral
            });
            
            Log("Вы отказались от архетипа и играете нейтрально");
            btnAcceptArchetype.Visible = false;
            btnDeclineArchetype.Visible = false;
        }
        
        private void HandleStartTurn(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<StartTurnDto>(message.Payload);
            if (dto == null) return;
            
            Log($"\n=== ВАШ ХОД === Цикл: {dto.Cycle}, Ход: {dto.Turn}");
            btnEndTurn.Visible = true;
        }
        
        private void HandleProductionResult(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<ProductionResultDto>(message.Payload);
            if (dto == null) return;
            
            Log("Произведено ресурсов:");
            foreach (var (resource, amount) in dto.Produced)
            {
                Log($"  {resource}: +{amount}");
            }
        }
        
        private void HandleState(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<StateDto>(message.Payload);
            if (dto == null) return;
            
            _currentState = dto;
            
            // Обновляем поле
            fieldControl.UpdateState(dto);
            
            // Обновляем информацию
            lblInfo.Text = $"Солдаты: {dto.Soldiers}\nОборона: {dto.Defense}%\nЗданий: {dto.Buildings.Count}";
            
            // Обновляем ресурсы
            var resourceText = "Ресурсы:\n";
            foreach (var (resource, amount) in dto.Resources.OrderBy(kv => kv.Key))
            {
                resourceText += $"{resource}: {amount}\n";
            }
            lblResources.Text = resourceText;
            
            Log("\nТекущее состояние обновлено");
        }
        
        private void OnCellClicked(int placeId)
        {
            _selectedCell = placeId;
            Log($"Выбрана клетка {placeId}");
        }
        
        private async void BtnBuild_Click(object? sender, EventArgs e)
        {
            if (_selectedCell < 0)
            {
                MessageBox.Show("Выберите клетку на поле");
                return;
            }
            
            var buildingType = (BuildingType)cmbBuildingType.SelectedItem;
            
            await _client.SendMessage(MessageType.BUILD, new BuildRequestDto
            {
                PlaceId = _selectedCell,
                Type = buildingType
            });
            
            Log($"Попытка построить {buildingType} на клетке {_selectedCell}");
        }
        
        private async void BtnUpgrade_Click(object? sender, EventArgs e)
        {
            if (_selectedCell < 0)
            {
                MessageBox.Show("Выберите клетку с зданием");
                return;
            }
            
            await _client.SendMessage(MessageType.UPGRADE, new UpgradeRequestDto
            {
                PlaceId = _selectedCell
            });
            
            Log($"Попытка улучшить здание на клетке {_selectedCell}");
        }
        
        private async void BtnMakeSoldiers_Click(object? sender, EventArgs e)
        {
            await _client.SendMessage(MessageType.MAKE_SOLDIERS, new MakeSoldiersRequestDto
            {
                Count = (int)numSoldiers.Value
            });
            
            Log($"Попытка произвести {numSoldiers.Value} солдат");
        }
        
        private async void BtnAttack_Click(object? sender, EventArgs e)
        {
            if (cmbAttackTarget.SelectedItem == null)
            {
                MessageBox.Show("Выберите цель атаки");
                return;
            }
            
            var targetId = (int)cmbAttackTarget.SelectedItem;
            
            await _client.SendMessage(MessageType.ATTACK, new AttackRequestDto
            {
                ToPlayerId = targetId,
                Soldiers = (int)numAttackSoldiers.Value
            });
            
            Log($"Атака на игрока {targetId} с {numAttackSoldiers.Value} солдатами");
        }
        
        private void HandleAttackTarget(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<AttackTargetDto>(message.Payload);
            if (dto == null) return;
            
            Log($"\n!!! ВЫ АТАКОВАНЫ !!!");
            Log($"Потери атакующего: {dto.Losses}");
            Log($"Выживших: {dto.Survivors}");
            if (dto.StolenResources.Count > 0)
            {
                Log("Украдено:");
                foreach (var (resource, amount) in dto.StolenResources)
                {
                    Log($"  {resource}: -{amount}");
                }
            }
        }
        
        private void HandleTurnEnded(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<TurnEndedDto>(message.Payload);
            if (dto == null) return;
            
            if (dto.PlayerId == _myPlayerId)
            {
                Log("Ваш ход завершен. Ожидание...");
                btnEndTurn.Visible = false;
            }
        }
        
        private void HandleGameEnd(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<GameEndDto>(message.Payload);
            if (dto == null) return;
            
            Log("\n=== ИГРА ЗАВЕРШЕНА ===");
            Log("Результаты:");
            for (int i = 0; i < dto.Results.Count; i++)
            {
                var result = dto.Results[i];
                Log($"{i + 1}. {result.Nickname}: {result.Points} очков");
            }
        }
        
        private void HandleResponse(NetworkMessage message)
        {
            var dto = JsonSerializer.Deserialize<ResponseDto>(message.Payload);
            if (dto == null) return;
            
            if (!dto.Success)
            {
                Log($"Ошибка: {dto.Message}");
            }
        }
        
        private async void BtnEndTurn_Click(object? sender, EventArgs e)
        {
            await _client.SendMessage(MessageType.END_TURN);
        }
        
        private void Log(string message)
        {
            txtLog.AppendText(message + "\n");
            txtLog.ScrollToCaret();
        }
        
        private string GetArchetypeName(ArchetypeType type)
        {
            return type switch
            {
                ArchetypeType.Greedy => "Жадина",
                ArchetypeType.Patron => "Меценат",
                ArchetypeType.Warrior => "Воин",
                ArchetypeType.Recruit => "Новобранец",
                ArchetypeType.Engineer => "Инженер",
                ArchetypeType.Alchemist => "Алхимик",
                ArchetypeType.Glutton => "Обжора",
                _ => "Нейтральный"
            };
        }
        
        private string GetArchetypeDescription(ArchetypeType type)
        {
            return type switch
            {
                ArchetypeType.Greedy => "+30% оборона, -20% очков",
                ArchetypeType.Patron => "+25% очков, -25% оборона",
                ArchetypeType.Warrior => "Атака игнорирует 20% обороны, солдат стоит 5 хлеб + 2 оружие",
                ArchetypeType.Recruit => "Солдат стоит 1 хлеб + 1 оружие, атака страдает от +20% обороны",
                ArchetypeType.Engineer => "Возврат 1-2 ресурсов при строительстве, -20% очков",
                ArchetypeType.Alchemist => "+25% очков за драгоценности, -30% за остальное",
                ArchetypeType.Glutton => "Солдат крадет 2 ресурса, стоит 6 хлеб + 1 оружие",
                _ => "Без модификаторов"
            };
        }
    }
}

