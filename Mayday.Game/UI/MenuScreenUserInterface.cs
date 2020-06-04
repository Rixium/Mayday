using System;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using Image = Myra.Graphics2D.UI.Image;
using Window = Yetiface.Engine.Utils.Window;

namespace Mayday.Game.UI
{
    public class MenuScreenUserInterface : IUserInterface, INetworkServerListener, INetworkClientListener, IWorldMakerListener
    {
        
        private readonly IScreenManager _screenManager;
        
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageParser _messageParser;
        private readonly MaydayMessagePackager _packager;
        
        private IWorldMaker _worldMaker;
        
        public MenuScreenUserInterface(INetworkManager networkManager, IScreenManager screenManager)
        {
            _networkManager = networkManager;
            _packager = new MaydayMessagePackager();
            
            _screenManager = screenManager;
            
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);

            var panel = new Panel();

            panel.Widgets.Add(new Image
            {
                ResizeMode = ImageResizeMode.KeepAspectRatio,
                Renderable = new TextureRegion(YetiGame.ContentManager.Load<Texture2D>("MainMenu\\planet")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 300,
                Height = 300
            });

            var buttonPanel = new VerticalStackPanel()
            {
                Background = new SolidBrush(Color.White * 0.5f),
                Padding = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            buttonPanel.Widgets.Add(new TextButton
            {
                Text = "Single Player",
                StyleName = "commodore-64",
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            
            buttonPanel.Widgets.Add(new TextButton
            {
                Text = "Multiplayer",
                StyleName = "commodore-64",
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            buttonPanel.Widgets.Add(new TextButton
            {
                Text = "Settings",
                StyleName = "commodore-64",
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            buttonPanel.Widgets.Add(new TextButton
            {
                Text = "Quit",
                StyleName = "commodore-64",
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            });

            
            panel.Widgets.Add(buttonPanel);
            
            Desktop.Root = panel;
        }

        private void ConnectToLobby(string lobbyId)
        {
            SteamMatchmaking.OnLobbyEntered += OnConnectedToLobby;
            SteamMatchmaking.JoinLobbyAsync(ulong.Parse(lobbyId));
        }


        private void OnConnectedToLobby(Lobby obj)
        {
            var ip = obj.GetData("ip");
            JoinServer(ip.Trim());
        }
        

        private async void StartNewGame()
        {
            var gameScreen = 
                _screenManager.AddScreen(new GameScreen(_networkManager));

            var world = await CreateWorld();
            
            gameScreen.SetWorld(world);

            gameScreen.AddPlayer(new Player
            {
                HeadId = 1,
                BodyId = 1,
                LegsId = 1
            }, true);


            _screenManager.ChangeScreen(gameScreen.Name);
        }

        private async Task<IGameWorld> CreateWorld()
        {
            _worldMaker = new WorldMaker()
                .SetWorldSize(200, 200);
            
            return await _worldMaker.Create(this);
        }

        private void HostGame()
        {
            _networkManager.CreateSession();
            StartNewGame();
        }

        private void JoinServer(string ipAddress)
        {
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
            _worldMaker = new NetworkWorldMaker(_networkManager);
            
            var world = await _worldMaker
                .Create(this);
            
            var gameScreen = new GameScreen(_networkManager);
            
            gameScreen.SetWorld(world);
            
            var player = gameScreen.AddPlayer(new Player
            {
                HeadId = 1
            }, true);

            var newPlayerPacket = new NewPlayerPacket
            {
                SteamId = SteamClient.SteamId,
                HeadId = player.HeadId,
                X = (int) player.X,
                Y = (int) player.Y
            };

            var package = _packager.Package(newPlayerPacket);
            _networkManager.SendMessage(package);
                
            _screenManager.AddScreen(gameScreen);
            _screenManager.ChangeScreen(gameScreen.Name);
        }

        public void Draw()
        {
            Desktop.Render();
        }

        public void Update()
        {
            
            _networkManager.Update();
        }

        public void AfterDraw()
        {
            
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
            
        }
        
    }

}