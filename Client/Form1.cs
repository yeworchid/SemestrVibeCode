using Common;
using Common.DTO;
using Common.Services;
using System.Net.Sockets;

namespace Client
{
    public partial class Form1 : Form
    {
        private TcpClient client = null!;
        private NetworkStream stream = null!;
        private int myId = 0;
        private int currentCycle = 0;
        private int currentTurn = 0;
        private bool myTurn = false;
        private Dictionary<string, int> resources = new Dictionary<string, int>();
        private List<BuildingStateDto> buildings = new List<BuildingStateDto>();
        private int soldiers = 0;
        private int defense = 0;
        private int selectedPlace = -1;
        private bool waitingForResponse = false;

        public Form1()
        {
            InitializeComponent();
            LoadArchetypes();
        }

        private void LoadArchetypes()
        {
            cmbArchetype.Items.Add("Жадина");
            cmbArchetype.Items.Add("Меценат");
            cmbArchetype.Items.Add("Воин");
            cmbArchetype.Items.Add("Новобранец");
            cmbArchetype.Items.Add("Инженер");
            cmbArchetype.Items.Add("Алхимик");
            cmbArchetype.Items.Add("Обжора");
            cmbArchetype.Items.Add("Нормис");
            cmbArchetype.SelectedIndex = 7;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();

                var dto = new JoinDto
                {
                    Nickname = txtNickname.Text,
                    Email = txtEmail.Text
                };
                SendMsg(MessageType.JOIN, dto);

                Task.Run(() => ReceiveLoop());

                btnConnect.Enabled = false;
                Log("Подключено к серверу");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void BtnSelectArchetype_Click(object sender, EventArgs e)
        {
            ArchetypeType arch = ArchetypeType.Neutral;
            switch (cmbArchetype.SelectedIndex)
            {
                case 0: arch = ArchetypeType.Greedy; break;
                case 1: arch = ArchetypeType.Patron; break;
                case 2: arch = ArchetypeType.Warrior; break;
                case 3: arch = ArchetypeType.Recruit; break;
                case 4: arch = ArchetypeType.Engineer; break;
                case 5: arch = ArchetypeType.Alchemist; break;
                case 6: arch = ArchetypeType.Glutton; break;
                case 7: arch = ArchetypeType.Neutral; break;
            }

            var dto = new ArchetypeDto { ArchetypeType = arch };
            SendMsg(MessageType.ARCHETYPE, dto);

            cmbArchetype.Visible = false;
            btnSelectArchetype.Visible = false;
            Log("Архетип выбран");
        }

        private void BtnBuild_Click(object sender, EventArgs e)
        {
            if (!myTurn)
            {
                MessageBox.Show("Не ваш ход");
                return;
            }

            if (waitingForResponse)
            {
                MessageBox.Show("Дождитесь ответа сервера");
                return;
            }

            if (selectedPlace < 0)
            {
                MessageBox.Show("Выберите место на поле");
                return;
            }

            var buildForm = new BuildForm();
            if (buildForm.ShowDialog() == DialogResult.OK)
            {
                var dto = new BuildRequestDto
                {
                    PlaceId = selectedPlace,
                    Type = buildForm.SelectedType
                };
                SendMsgWithWait(MessageType.BUILD, dto);
            }
        }

        private void BtnUpgrade_Click(object sender, EventArgs e)
        {
            if (!myTurn)
            {
                MessageBox.Show("Не ваш ход");
                return;
            }

            if (waitingForResponse)
            {
                MessageBox.Show("Дождитесь ответа сервера");
                return;
            }

            if (lstBuildings.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите здание");
                return;
            }

            var b = buildings[lstBuildings.SelectedIndex];
            var dto = new UpgradeRequestDto { PlaceId = b.PlaceId };
            SendMsgWithWait(MessageType.UPGRADE, dto);
        }

        private void BtnMakeSoldiers_Click(object sender, EventArgs e)
        {
            if (!myTurn)
            {
                MessageBox.Show("Не ваш ход");
                return;
            }

            if (waitingForResponse)
            {
                MessageBox.Show("Дождитесь ответа сервера");
                return;
            }

            BuildingStateDto? barracks = null;
            foreach (var b in buildings)
            {
                if (b.Type == BuildingType.Barracks)
                {
                    barracks = b;
                    break;
                }
            }

            if (barracks == null)
            {
                MessageBox.Show("Нет казарм");
                return;
            }

            string input = Microsoft.VisualBasic.Interaction.InputBox("Сколько солдат?", "Создать солдат", "1");
            if (int.TryParse(input, out int count))
            {
                var dto = new MakeSoldiersRequestDto
                {
                    BarracksId = barracks.PlaceId,
                    Count = count
                };
                SendMsgWithWait(MessageType.MAKE_SOLDIERS, dto);
            }
        }

        private void BtnAttack_Click(object sender, EventArgs e)
        {
            if (!myTurn)
            {
                MessageBox.Show("Не ваш ход");
                return;
            }

            if (waitingForResponse)
            {
                MessageBox.Show("Дождитесь ответа сервера");
                return;
            }

            string targetStr = Microsoft.VisualBasic.Interaction.InputBox("ID цели:", "Атака", "2");
            string countStr = Microsoft.VisualBasic.Interaction.InputBox("Сколько солдат?", "Атака", "1");

            if (int.TryParse(targetStr, out int targetId) && int.TryParse(countStr, out int count))
            {
                var dto = new AttackRequestDto
                {
                    ToPlayerId = targetId,
                    Soldiers = count
                };
                SendMsgWithWait(MessageType.ATTACK, dto);
            }
        }

        private void BtnEndTurn_Click(object sender, EventArgs e)
        {
            if (!myTurn)
            {
                MessageBox.Show("Не ваш ход");
                return;
            }

            SendMsg(MessageType.END_TURN, new { });
            myTurn = false;
            btnEndTurn.Enabled = false;
        }

        private void GameField_PlaceClicked(object sender, int placeId)
        {
            selectedPlace = placeId;
        }

        private void SendMsg(MessageType type, object payload)
        {
            try
            {
                var msg = MessageSerializer.Serialize(type, payload);
                string str = MessageParser.Serialize(msg);
                byte[] data = ByteConverter.StringToBytes(str);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log("Ошибка: " + ex.Message);
            }
        }

        private void SendMsgWithWait(MessageType type, object payload)
        {
            waitingForResponse = true;
            lblWaiting.Text = "Ожидание ответа...";
            lblWaiting.Visible = true;
            SendMsg(type, payload);
        }

        private void ClearWaiting()
        {
            waitingForResponse = false;
            lblWaiting.Visible = false;
        }

        private void ReceiveLoop()
        {
            byte[] buffer = new byte[8192];
            int offset = 0;

            try
            {
                while (true)
                {
                    int read = stream.Read(buffer, offset, buffer.Length - offset);
                    if (read == 0) break;
                    offset += read;

                    while (ByteConverter.TryReadMessage(buffer, 0, offset, out string msgStr, out int bytesUsed))
                    {
                        var msg = MessageParser.Parse(msgStr);
                        HandleMsg(msg);

                        Buffer.BlockCopy(buffer, bytesUsed, buffer, 0, offset - bytesUsed);
                        offset -= bytesUsed;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка приема: " + ex.Message);
            }
        }

        private void HandleMsg(NetworkMessage msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                switch (msg.Type)
                {
                    case MessageType.RESPONSE:
                        var resp = MessageDeserializer.Deserialize<ResponseDto>(msg);
                        if (resp != null)
                            Log(resp.Message ?? "");
                        ClearWaiting();
                        break;

                    case MessageType.START_GAME:
                        var start = MessageDeserializer.Deserialize<StartGameDto>(msg);
                        if (start != null)
                        {
                            myId = start.PlayerId;
                            Log("Игра началась! Вы игрок " + myId);

                            if (start.Players != null)
                            {
                                Log("Игроки:");
                                foreach (var pl in start.Players)
                                {
                                    Log("  " + pl.Id + ": " + pl.Nickname);
                                }
                            }

                            cmbArchetype.Visible = true;
                            btnSelectArchetype.Visible = true;
                        }
                        break;

                    case MessageType.START_TURN:
                        var turn = MessageDeserializer.Deserialize<StartTurnDto>(msg);
                        if (turn != null)
                        {
                            currentCycle = turn.Cycle;
                            currentTurn = turn.Turn;
                            myTurn = (turn.PlayerId == myId);

                            lblCycle.Text = "Цикл: " + currentCycle;
                            lblTurn.Text = "Ход: " + currentTurn;

                            if (myTurn)
                            {
                                Log("Ваш ход!");
                                btnEndTurn.Enabled = true;
                                pnlGame.Visible = true;
                                AnimateTurnStart();
                            }
                            else
                            {
                                Log("Ход игрока " + turn.PlayerId);
                            }
                        }
                        break;

                    case MessageType.STATE:
                        var state = MessageDeserializer.Deserialize<StateDto>(msg);
                        if (state != null)
                            UpdateState(state);
                        break;

                    case MessageType.PRODUCTION_RESULT:
                        var prod = MessageDeserializer.Deserialize<ProductionResultDto>(msg);
                        if (prod != null && prod.ProducedResources != null)
                        {
                            string prodStr = "";
                            foreach (var r in prod.ProducedResources)
                                prodStr += r.Key + ":" + r.Value + " ";
                            Log("Произведено: " + prodStr);
                        }
                        break;

                    case MessageType.ATTACK_TARGET:
                        var atk = MessageDeserializer.Deserialize<AttackTargetDto>(msg);
                        if (atk != null)
                        {
                            string stolenStr = "";
                            if (atk.StolenResources != null)
                            {
                                foreach (var r in atk.StolenResources)
                                    stolenStr += r.Key + ":" + r.Value + " ";
                            }
                            Log("Атака: отправлено " + atk.Sent + ", потеряно " + atk.Lost + ", украдено: " + stolenStr);
                            AnimateAttack();
                        }
                        break;

                    case MessageType.TURN_ENDED:
                        var ended = MessageDeserializer.Deserialize<TurnEndedDto>(msg);
                        if (ended != null)
                            Log("Ход завершен. Следующий: " + ended.NextPlayerId);
                        break;

                    case MessageType.GAME_END:
                        var end = MessageDeserializer.Deserialize<GameEndDto>(msg);
                        if (end != null)
                        {
                            string winMsg = "=== ИГРА ОКОНЧЕНА ===\n\n";
                            winMsg += "Результаты:\n";

                            if (end.AllScores != null)
                            {
                                foreach (var score in end.AllScores)
                                {
                                    winMsg += score.Nickname + ": " + score.Points + " очков\n";
                                }
                            }

                            winMsg += "\nПобедитель: игрок " + end.WinnerPlayerId;
                            winMsg += "\nОчки победителя: " + end.Points;

                            MessageBox.Show(winMsg, "Конец игры");
                        }
                        break;
                }
            });
        }

        private void UpdateState(StateDto state)
        {
            resources = state.Resources ?? new Dictionary<string, int>();
            soldiers = state.Soldiers;
            defense = state.Defense;
            buildings = state.Buildings ?? new List<BuildingStateDto>();

            string resStr = "Ресурсы:\n";
            foreach (var r in resources)
                resStr += r.Key + ": " + r.Value + "\n";
            resStr += "\nСолдаты: " + soldiers;
            resStr += "\nЗащита: " + defense + "%";
            lblResources.Text = resStr;

            lstBuildings.Items.Clear();
            foreach (var b in buildings)
            {
                lstBuildings.Items.Add("Место " + b.PlaceId + ": " + b.Type + " ур." + b.Level);
            }

            gameField.UpdateBuildings(buildings);
        }

        private void AnimateTurnStart()
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            int step = 0;
            Color origColor = pnlGame.BackColor;

            timer.Tick += (s, e) =>
            {
                step++;
                if (step <= 5)
                {
                    pnlGame.BackColor = Color.LightGreen;
                }
                else if (step <= 10)
                {
                    pnlGame.BackColor = origColor;
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private void AnimateAttack()
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            int step = 0;
            Color origColor = pnlGame.BackColor;

            timer.Tick += (s, e) =>
            {
                step++;
                if (step % 2 == 1)
                {
                    pnlGame.BackColor = Color.Red;
                }
                else
                {
                    pnlGame.BackColor = origColor;
                }

                if (step >= 6)
                {
                    pnlGame.BackColor = origColor;
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private void Log(string msg)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke((MethodInvoker)delegate { Log(msg); });
            }
            else
            {
                txtLog.AppendText(msg + "\r\n");
            }
        }
    }
}
