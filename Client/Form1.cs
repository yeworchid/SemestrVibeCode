using Common;
using Common.DTO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private int myPlayerId;
        private int currentCycle;
        private int currentTurn;
        private bool isMyTurn = false;
        private Dictionary<Resources, int> resources = new Dictionary<Resources, int>();
        private List<BuildingStateDto> buildings = new List<BuildingStateDto>();
        private int soldiers = 0;
        private int defense = 0;

        public Form1()
        {
            InitializeComponent();
            LoadArchetypes();
        }

        private void LoadArchetypes()
        {
            cmbArchetype.Items.Add("Greedy");
            cmbArchetype.Items.Add("Patron");
            cmbArchetype.Items.Add("Warrior");
            cmbArchetype.Items.Add("Recruit");
            cmbArchetype.Items.Add("Engineer");
            cmbArchetype.Items.Add("Alchemist");
            cmbArchetype.Items.Add("Glutton");
            cmbArchetype.Items.Add("Neutral");
            cmbArchetype.SelectedIndex = 7;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();

                var joinDto = new JoinDto
                {
                    Nickname = txtNickname.Text,
                    Email = txtEmail.Text
                };

                SendMessage(MessageType.JOIN, joinDto);
                Task.Run(() => ReceiveMessages());

                btnConnect.Enabled = false;
                Log("Connected to server");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error: " + ex.Message);
            }
        }

        private void BtnSelectArchetype_Click(object sender, EventArgs e)
        {
            ArchetypeType archetype = ArchetypeType.Neutral;
            switch (cmbArchetype.SelectedIndex)
            {
                case 0: archetype = ArchetypeType.Greedy; break;
                case 1: archetype = ArchetypeType.Patron; break;
                case 2: archetype = ArchetypeType.Warrior; break;
                case 3: archetype = ArchetypeType.Recruit; break;
                case 4: archetype = ArchetypeType.Engineer; break;
                case 5: archetype = ArchetypeType.Alchemist; break;
                case 6: archetype = ArchetypeType.Glutton; break;
                case 7: archetype = ArchetypeType.Neutral; break;
            }

            var archetypeDto = new ArchetypeDto { ArchetypeType = archetype };
            SendMessage(MessageType.ARCHETYPE, archetypeDto);

            cmbArchetype.Visible = false;
            btnSelectArchetype.Visible = false;
            Log("Archetype selected: " + archetype);
        }


        private void BtnBuild_Click(object sender, EventArgs e)
        {
            if (!isMyTurn)
            {
                MessageBox.Show("Not your turn");
                return;
            }

            var buildForm = new BuildForm();
            if (buildForm.ShowDialog() == DialogResult.OK)
            {
                var buildDto = new BuildRequestDto
                {
                    PlaceId = buildForm.PlaceId,
                    Type = buildForm.BuildingType
                };
                SendMessage(MessageType.BUILD, buildDto);
            }
        }

        private void BtnUpgrade_Click(object sender, EventArgs e)
        {
            if (!isMyTurn)
            {
                MessageBox.Show("Not your turn");
                return;
            }

            if (lstBuildings.SelectedItem == null)
            {
                MessageBox.Show("Select a building");
                return;
            }

            var building = buildings[lstBuildings.SelectedIndex];
            var upgradeDto = new UpgradeRequestDto { PlaceId = building.PlaceId };
            SendMessage(MessageType.UPGRADE, upgradeDto);
        }

        private void BtnMakeSoldiers_Click(object sender, EventArgs e)
        {
            if (!isMyTurn)
            {
                MessageBox.Show("Not your turn");
                return;
            }

            var barracks = buildings.FirstOrDefault(b => b.Type == BuildingType.Barracks);
            if (barracks == null)
            {
                MessageBox.Show("No barracks");
                return;
            }

            string input = Microsoft.VisualBasic.Interaction.InputBox("How many soldiers?", "Make Soldiers", "1");
            if (int.TryParse(input, out int count))
            {
                var soldiersDto = new MakeSoldiersRequestDto
                {
                    BarracksId = barracks.PlaceId,
                    Count = count
                };
                SendMessage(MessageType.MAKE_SOLDIERS, soldiersDto);
            }
        }

        private void BtnAttack_Click(object sender, EventArgs e)
        {
            if (!isMyTurn)
            {
                MessageBox.Show("Not your turn");
                return;
            }

            string targetInput = Microsoft.VisualBasic.Interaction.InputBox("Target player ID:", "Attack", "2");
            string soldiersInput = Microsoft.VisualBasic.Interaction.InputBox("How many soldiers?", "Attack", "1");

            if (int.TryParse(targetInput, out int targetId) && int.TryParse(soldiersInput, out int soldierCount))
            {
                var attackDto = new AttackRequestDto
                {
                    ToPlayerId = targetId,
                    Soldiers = soldierCount
                };
                SendMessage(MessageType.ATTACK, attackDto);
            }
        }

        private void BtnEndTurn_Click(object sender, EventArgs e)
        {
            if (!isMyTurn)
            {
                MessageBox.Show("Not your turn");
                return;
            }

            SendMessage(MessageType.END_TURN, new { });
            isMyTurn = false;
            btnEndTurn.Enabled = false;
        }

        private void SendMessage(MessageType type, object payload)
        {
            try
            {
                var msg = new NetworkMessage
                {
                    Type = type,
                    Payload = JsonSerializer.Serialize(payload)
                };
                string json = JsonSerializer.Serialize(msg);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log("Send error: " + ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[8192];
            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var msg = JsonSerializer.Deserialize<NetworkMessage>(json);
                    HandleMessage(msg);
                }
            }
            catch (Exception ex)
            {
                Log("Receive error: " + ex.Message);
            }
        }

        private void HandleMessage(NetworkMessage msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                switch (msg.Type)
                {
                    case MessageType.RESPONSE:
                        var response = JsonSerializer.Deserialize<ResponseDto>(msg.Payload);
                        Log(response.Message);
                        break;

                    case MessageType.START_GAME:
                        var startGame = JsonSerializer.Deserialize<StartGameDto>(msg.Payload);
                        myPlayerId = startGame.PlayerId;
                        Log($"Game started! You are player {myPlayerId}");
                        cmbArchetype.Visible = true;
                        btnSelectArchetype.Visible = true;
                        break;

                    case MessageType.START_TURN:
                        var startTurn = JsonSerializer.Deserialize<StartTurnDto>(msg.Payload);
                        currentCycle = startTurn.Cycle;
                        currentTurn = startTurn.Turn;
                        isMyTurn = (startTurn.PlayerId == myPlayerId);
                        
                        lblCycle.Text = $"Cycle: {currentCycle}";
                        lblTurn.Text = $"Turn: {currentTurn}";
                        
                        if (isMyTurn)
                        {
                            Log("Your turn!");
                            btnEndTurn.Enabled = true;
                            pnlGame.Visible = true;
                        }
                        else
                        {
                            Log($"Player {startTurn.PlayerId}'s turn");
                        }
                        break;

                    case MessageType.STATE:
                        var state = JsonSerializer.Deserialize<StateDto>(msg.Payload);
                        UpdateState(state);
                        break;

                    case MessageType.PRODUCTION_RESULT:
                        var production = JsonSerializer.Deserialize<ProductionResultDto>(msg.Payload);
                        Log("Production: " + string.Join(", ", production.ProducedResources.Select(r => $"{r.Key}:{r.Value}")));
                        break;

                    case MessageType.ATTACK_TARGET:
                        var attack = JsonSerializer.Deserialize<AttackTargetDto>(msg.Payload);
                        Log($"Attack result: Sent {attack.Sent}, Lost {attack.Lost}, Stolen: {string.Join(", ", attack.StolenResources.Select(r => $"{r.Key}:{r.Value}"))}");
                        break;

                    case MessageType.TURN_ENDED:
                        var turnEnded = JsonSerializer.Deserialize<TurnEndedDto>(msg.Payload);
                        Log($"Turn ended. Next: Player {turnEnded.NextPlayerId}");
                        break;

                    case MessageType.GAME_END:
                        var gameEnd = JsonSerializer.Deserialize<GameEndDto>(msg.Payload);
                        MessageBox.Show($"Game Over! Winner: Player {gameEnd.WinnerPlayerId} with {gameEnd.Points} points");
                        break;
                }
            });
        }

        private void UpdateState(StateDto state)
        {
            resources.Clear();
            foreach (var r in state.Resources)
            {
                if (Enum.TryParse<Resources>(r.Key, out var res))
                {
                    resources[res] = r.Value;
                }
            }

            soldiers = state.Soldiers;
            defense = state.Defense;
            buildings = state.Buildings;

            lblResources.Text = "Resources:\n" + string.Join("\n", resources.Select(r => $"{r.Key}: {r.Value}")) + 
                                $"\n\nSoldiers: {soldiers}\nDefense: {defense}%";

            lstBuildings.Items.Clear();
            foreach (var b in buildings)
            {
                lstBuildings.Items.Add($"Place {b.PlaceId}: {b.Type} Lv{b.Level}");
            }

            gameField.UpdateBuildings(buildings);
        }

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke((MethodInvoker)delegate { Log(message); });
            }
            else
            {
                txtLog.AppendText(message + "\r\n");
            }
        }
    }
}
