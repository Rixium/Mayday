using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Animators;
using GeonBit.UI.Entities;
using GeonBit.UI.Utils;
using Mayday.Game.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using Image = GeonBit.UI.Entities.Image;

namespace Mayday.Game.UI
{
    public class MenuScreenUserInterface : IUserInterface, INetworkServerListener, INetworkClientListener
    {
        public HashSet<string> Connections { get; set; } = new HashSet<string>();

        
        private readonly INetworkManager _networkManager;
        private readonly UserInterface _active;
        
        private readonly Entity _rootPanel;
        private Entity _mainMenuButtonPanel;
        private Entity _singlePlayerPanel;
        private Entity _multiplayerPanel;
        private Entity _hostGamePanel;
        private Entity _joinGamePanel;
        private Entity _settingsPanel;
        
        private readonly SoundEffect _clickSound;
        private readonly SoundEffect _hoverSound;
        private Panel _userList;
        private Panel _connectedToServerPanel;
        private Entity _chatBox;
        private Entity _joinServerPanel;

        public MenuScreenUserInterface(INetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _hoverSound = YetiGame.ContentManager.Load<SoundEffect>("MainMenu/tap");
            _clickSound = YetiGame.ContentManager.Load<SoundEffect>("MainMenu/click");

            UserInterface.Initialize(YetiGame.ContentManager);
            
            UserInterface.Active.UseRenderTarget = true;

            _active = UserInterface.Active;
            
            _rootPanel =  UserInterface.Active.AddEntity(new Panel(new Vector2(0, 0))
            {
                FillColor = Color.Black
            });

            _rootPanel.Padding = new Vector2(30, 30);

            var centerPanel = _rootPanel.AddChild(new Panel(new Vector2(0.5f, 0.5f), PanelSkin.None));
            
            var image = centerPanel.AddChild(new Image(YetiGame.ContentManager.Load<Texture2D>("MainMenu/planet"), new Vector2(0.8f, 0.8f), ImageDrawMode.Stretch, Anchor.Center)
            {
                EnforceSquare = true
            });
            
            image.SpaceBefore = new Vector2(100, 100);
            image.SpaceAfter =  new Vector2(100, 100);


            image.AttachAnimator(new RotationAnimator());
            
            var titlePanel = _rootPanel.AddChild(new Panel(new Vector2(0, 0.1f), PanelSkin.None, Anchor.TopCenter));
            titlePanel.AddChild(new Header("Mayday", Anchor.Center)
            {
                FillColor = Color.White,
                OutlineColor = Color.Black
            });

            SetupMainMenuPanel();
            SetupSinglePlayerPanel();
            SetupMultiplayerPanel();
            SetupHostGamePanel();
            SetupJoinGamePanel();
            SetupSettingsPanel();
            
            SetupFooter();

            var stack = new Stack<Entity>();
            stack.Push(_rootPanel);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var child in current.Children)
                {
                    stack.Push(child);
                    
                    if (!(child is Button)) continue;
                
                    child.OnMouseEnter += OnMouseEnter;
                    child.OnClick += OnMouseClick;
                }
            }
            
            var args = Environment.GetCommandLineArgs();
            int num = 0;
            
            foreach (var param in args)
            {
                if (param.Equals("+connect_lobby"))
                {
                    ConnectToLobby(args[num + 1]);
                    break;
                }
                num++;
            }
        }

        private void ConnectToLobby(string lobbyId)
        {
            _mainMenuButtonPanel.Visible = false;
            SteamMatchmaking.OnLobbyEntered += OnConnectedToLobby;
            SteamMatchmaking.JoinLobbyAsync(ulong.Parse(lobbyId));
        }


        private void OnConnectedToLobby(Lobby obj)
        {
            var ip = obj.GetData("ip");
            _rootPanel.AddChild(new Paragraph(ip));
            JoinServer(ip);
        }

        private void OnMouseClick(Entity entity) => _clickSound.Play();

        private void OnMouseEnter(Entity entity) => _hoverSound.Play();

        private void SetupMainMenuPanel()
        {
            _mainMenuButtonPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None));
            var singlePlayerButton = _mainMenuButtonPanel.AddChild(new Button("Single Player"));
            var multiplayerButton = _mainMenuButtonPanel.AddChild(new Button("Multiplayer"));
            var settingsButton = _mainMenuButtonPanel.AddChild(new Button("Settings"));
            var exitButton = _mainMenuButtonPanel.AddChild(new Button("Exit"));
            
            singlePlayerButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = false;
                _singlePlayerPanel.Visible = true;
            };
            
            multiplayerButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = false;
                _multiplayerPanel.Visible = true;
            };
            
            settingsButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = false;
                _settingsPanel.Visible = true;
            };
            
            exitButton.OnClick += (e) =>
            {
                YetiGame.Quit();
            };
        }

        private void SetupSinglePlayerPanel()
        {
            _singlePlayerPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None)
            {
                Visible = false
            });
            
            _singlePlayerPanel.AddChild(new Button("New Game"));
            _singlePlayerPanel.AddChild(new Button("Load Game"));
            var backButton = _singlePlayerPanel.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _singlePlayerPanel.Visible = false;
            };
        }
        
        private void SetupMultiplayerPanel()
        {
            _multiplayerPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None)
            {
                Visible = false
            });
            
            var hostGameButton = _multiplayerPanel.AddChild(new Button("Host Game"));
            var joinGameButton = _multiplayerPanel.AddChild(new Button("Join Game"));
            var backButton = _multiplayerPanel.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _multiplayerPanel.Visible = false;
            };
                        
            hostGameButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = false;
                _hostGamePanel.Visible = true;
            };      
            
            joinGameButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = false;
                _joinGamePanel.Visible = true;
            };
        }

        private void SetupHostGamePanel()
        {
            _hostGamePanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None)
            {
                Visible = false
            });
            
            var newGameButton = _hostGamePanel.AddChild(new Button("New Game"));
            _hostGamePanel.AddChild(new Button("Load Game"));
            var backButton = _hostGamePanel.AddChild(new Button("Back"));

            newGameButton.OnClick += HostGame;
            
            backButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = true;
                _hostGamePanel.Visible = false;
            };
        }

        private void HostGame(Entity entity)
        {
            _networkManager.CreateSession();
            _hostGamePanel.Visible = false;
            OnConnectedToServer(new ConnectionInfo());
        }

        private void SetupJoinGamePanel()
        {
            _joinGamePanel = _rootPanel.AddChild(new Panel(new Vector2(0, 0), PanelSkin.None)
            {
                Visible = false
            });

            var joinGamePanelButtons =
                _joinGamePanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None));
            
            var joinByIp = joinGamePanelButtons.AddChild(new Button("Join By IP"));
            var joinSteamFriend = joinGamePanelButtons.AddChild(new Button("Join Steam Friend"));
            var backButton = joinGamePanelButtons.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = true;
                _joinGamePanel.Visible = false;
            };

            joinByIp.OnClick += async (e) =>
            {
                var textInput = new TextInput(false)
                {
                    PlaceholderText = "Enter IP:"
                };

                var result = false;
                
                MessageBox.ShowMsgBox("Join Server by IP", "Please enter the IP address of the server you wish to join.", new[] {
                    new MessageBox.MsgBoxOption("Join", () =>
                    {
                        result = JoinServer(textInput.Value);
                        return true;
                    })
                }, new Entity[] { textInput });
                
                if(result)
                    await WaitToConnect();
            };
                
            joinSteamFriend.OnClick += (e) =>
            {
                SteamFriends.OpenOverlay("friends");
                
                _multiplayerPanel.Visible = true;
                _joinGamePanel.Visible = false;
            };
        }

        private bool JoinServer(string ipAddress)
        {
            _joinGamePanel.Visible = false;

            _joinServerPanel = _rootPanel.AddChild(new Panel(new Vector2(400, 0.5f)));
            var text = _joinServerPanel.AddChild(new RichParagraph("Joining...", Anchor.Center));
            
            text.AttachAnimator(new TextWaveAnimator());

            try
            {
                _networkManager.JoinSession(ipAddress);
            }
            catch (Exception ex)
            {
                // Possibly a problem with the IP ADDRESS
                // Go straight to failed.
                OnFailedToConnect();
                
                return false;
            }

            return true;
        }

        private void SetupSettingsPanel()
        {
            _settingsPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None)
            {
                Visible = false
            });
            
            var resolutionButton = _settingsPanel.AddChild(new Button("Change Resolution"));
            var backButton = _settingsPanel.AddChild(new Button("Back"));
            
            resolutionButton.OnClick += (e) => Game1.NextResize();
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _settingsPanel.Visible = false;
            };
        }

        private void SetupFooter()
        {
            var bottomText = _rootPanel.AddChild(new RichParagraph("YetiFace", Anchor.BottomCenter)
            {
                Scale = 0.8f
            });
            
            bottomText.AttachAnimator(new TextWaveAnimator());
        }

        public void Draw() => _active.Draw(GraphicsUtils.Instance.SpriteBatch);

        public void Update()
        {
            _active.Update(Time.GameTime);
            
            _networkManager.Update();
        }

        public void AfterDraw()
        {
            UserInterface.Active.DrawMainRenderTarget(GraphicsUtils.Instance.SpriteBatch);
        }


        public void OnNewConnection(Connection connection, ConnectionInfo info)
        {
            var steamName = SteamFriends.GetFriendPersona(info.Identity.SteamId);
            Connections.Add(steamName);
            
            UpdateUserList();
        }

        private void UpdateUserList()
        {

            return;
            
            if(_userList != null)
                _rootPanel.RemoveChild(_userList);
                    
            _userList = new Panel(new Vector2(0.3f, 0.8f), PanelSkin.Default, Anchor.CenterLeft);
            _rootPanel.AddChild(_userList);

            foreach (var user in Connections)
                _userList.AddChild(new Paragraph(user));
        }
        
        private async Task WaitToConnect()
        {
            // Give them 10 seconds to try to join.
            await Task.Delay(10000);
            
            if (_networkManager.Client == null)
            {
                OnFailedToConnect();
            } else if (!_networkManager.Client.Connected)
            {
                OnFailedToConnect();
            }
        }

        private void OnFailedToConnect()
        {
            if (_joinServerPanel != null)
                _rootPanel.RemoveChild(_joinServerPanel);

            _joinGamePanel.Visible = true;
            
            MessageBox.ShowMsgBox("Failed!", "Failed to join server!", new[]
            {
                new MessageBox.MsgBoxOption("Close", () => true)
            });
        }

        public void OnConnectionLeft(Connection connection, ConnectionInfo info)
        {
            var steamName = SteamFriends.GetFriendPersona(info.Identity.SteamId);
            Connections.Remove(steamName);
            
            UpdateUserList();
        }

        public void OnMessageReceived(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
            long recvTime, int channel)
        {
            var result = new NetworkMessageParser().Parse(data, size);

            if (result is Message message)
            {
                var steamName = SteamFriends.GetFriendPersona(message.SteamUserId);
                AddMessageToChatBox($"{steamName}: {message.Text}");
            }
        }

        public void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var result = new NetworkMessageParser().Parse(data, size);

            if (result is Message message)
            {
                var steamName = SteamFriends.GetFriendPersona(message.SteamUserId);
                AddMessageToChatBox($"{steamName}: {message.Text}");
            }
            
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            if(_connectedToServerPanel != null)
                _rootPanel.RemoveChild(_connectedToServerPanel);
            
            _connectedToServerPanel = new Panel(new Vector2(0.8f, 0.7f));
            _rootPanel.AddChild(_connectedToServerPanel);

            _connectedToServerPanel.AddChild(new Header("Connected to Server!"));
            _chatBox = _connectedToServerPanel.AddChild(new Panel(new Vector2(0, 0.5f), PanelSkin.Default, Anchor.TopCenter));
            ((Panel) _chatBox).PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;

            var chatPanel =
                _connectedToServerPanel.AddChild(new Panel(new Vector2(-1, -1), PanelSkin.None, Anchor.BottomCenter));
            var text = chatPanel.AddChild(new TextInput(false));
            var button = chatPanel.AddChild(new Button("Send"));

            button.OnClick += (e) =>
            {
                var textValue = ((TextInput) text).Value;
                _networkManager.SendMessage(textValue);

                var message = $"{SteamFriends.GetPersona()}: {textValue}";
                AddMessageToChatBox(message);
                
                ((TextInput) text).Value = "";
            };
        }

        private void AddMessageToChatBox(string message) => _chatBox.AddChild(new Paragraph(message));
        
    }

}