using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Animators;
using GeonBit.UI.Entities;
using GeonBit.UI.Utils;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using Image = GeonBit.UI.Entities.Image;

namespace Mayday.Game.UI
{
    public class MenuScreenUserInterface : IUserInterface, INetworkServerListener, INetworkClientListener, IWorldMakerListener
    {
        
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageParser _messageParser;
        private IWorldMaker _worldMaker;
        
        private readonly IScreenManager _screenManager;
        private readonly UserInterface _active;
        
        private readonly Entity _rootPanel;
        private Entity _mainMenuButtonPanel;
        private Entity _singlePlayerPanel;
        private Entity _multiplayerPanel;
        private Entity _hostGamePanel;
        private Entity _joinGamePanel;
        private Entity _settingsPanel;
        private Entity _joinServerPanel;
        private Entity _worldCreationParagraph;
        
        private readonly SoundEffect _clickSound;
        private readonly SoundEffect _hoverSound;

        public MenuScreenUserInterface(INetworkManager networkManager, IScreenManager screenManager)
        {
            _networkManager = networkManager;
            _screenManager = screenManager;
            
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
            JoinServer(ip.Trim());
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
            
            var newGameButton = _singlePlayerPanel.AddChild(new Button("New Game"));
            _singlePlayerPanel.AddChild(new Button("Load Game"));
            var backButton = _singlePlayerPanel.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _singlePlayerPanel.Visible = false;
            };

            newGameButton.OnClick += (e) =>
            {
                _singlePlayerPanel.Visible = false;
                StartNewGame();
            };
        }

        private async void StartNewGame()
        {
            _mainMenuButtonPanel.Visible = false;
            
            var creatingWorldPanel = _rootPanel.AddChild(new Panel(new Vector2(400, 0.5f)));
            _worldCreationParagraph = creatingWorldPanel.AddChild(new RichParagraph("Creating World...", Anchor.Center));
            _worldCreationParagraph.AttachAnimator(new TextWaveAnimator());

            var gameScreen = new GameScreen(_networkManager);

            var world = await CreateWorld();
            
            gameScreen.SetWorld(world);
            
            _screenManager.AddScreen(gameScreen);
            _screenManager.ChangeScreen(gameScreen.Name);
        }

        private async Task<IGameWorld> CreateWorld()
        {
            _worldMaker = new WorldMaker()
                .SetWorldSize(200, 200);
            
            return await _worldMaker.Create(this);
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
            StartNewGame();
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

            joinByIp.OnClick += (e) =>
            {
                var textInput = new TextInput(false)
                {
                    PlaceholderText = "Enter IP:"
                };

                MessageBox.ShowMsgBox("Join Server by IP",
                    "Please enter the IP address of the server you wish to join.", new[]
                    {
                        new MessageBox.MsgBoxOption("Join", () =>
                        {
                            JoinServer(textInput.Value);
                            return true;
                        })
                    }, new Entity[] {textInput});
            };
                
            joinSteamFriend.OnClick += (e) =>
            {
                SteamFriends.OpenOverlay("friends");
                
                _multiplayerPanel.Visible = true;
                _joinGamePanel.Visible = false;
            };
        }

        private void JoinServer(string ipAddress)
        {
            _joinGamePanel.Visible = false;

            _joinServerPanel = _rootPanel.AddChild(new Panel(new Vector2(400, 0.5f)));
            var text = _joinServerPanel.AddChild(new RichParagraph("Joining...", Anchor.Center));
            
            text.AttachAnimator(new TextWaveAnimator());

            try
            {
                _networkManager.JoinSession(ipAddress);
            }
            catch (Exception)
            {
                // Possibly a problem with the IP ADDRESS
                // Go straight to failed.
                OnFailedToConnect();
                return;
            }

            WaitToConnect();
        }

        private async void CreateNetworkWorld()
        {
            var creatingWorldPanel = _rootPanel.AddChild(new Panel(new Vector2(400, 0.5f)));
            _worldCreationParagraph = creatingWorldPanel.AddChild(new RichParagraph("Creating World...", Anchor.Center));
            _worldCreationParagraph.AttachAnimator(new TextWaveAnimator());
            
            _worldMaker = new NetworkWorldMaker(_networkManager);
            
            var world = await _worldMaker
                .Create(this);
            
            var gameScreen = new GameScreen(_networkManager);
            
            gameScreen.SetWorld(world);

            _screenManager.AddScreen(gameScreen);
            _screenManager.ChangeScreen(gameScreen.Name);
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

        public void Draw()
        {
            _active.Draw(GraphicsUtils.Instance.SpriteBatch);
        }

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
            
        }

        private async void WaitToConnect()
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
            
        }

        public void OnMessageReceived(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
            long recvTime, int channel)
        {
            var result = _messageParser.Parse(data, size);
            
        }

        public void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            CreateNetworkWorld();
        }

        public void OnWorldGenerationUpdate(string message)
        {
            ((RichParagraph) _worldCreationParagraph).Text = message;
        }
        
    }

}