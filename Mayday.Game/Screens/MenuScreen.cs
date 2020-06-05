using System;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Networking.SteamNetworking;
using Mayday.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Color = Microsoft.Xna.Framework.Color;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen,  INetworkServerListener, INetworkClientListener, IWorldMakerListener
    {
        private INetworkManager _networkManager;
        private IWorldMaker _worldMaker;
        
        private INetworkMessageParser _messageParser;
        private MaydayMessagePackager _packager;
        
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = new Color(4,13,21);
        }

        public override void Awake()
        {
            _networkManager = new SteamNetworkManager(Game1.AppId);        
            _packager = new MaydayMessagePackager();
            
            var panel = new MainMenuUserInterface();
            new MainMenuUserInterfaceController(panel);
            UserInterface = new MyraUserInterface(panel);
            
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            panel.StartGameNewGame.Click += (o, e) => 
                StartNewGame();
            
            panel.MultiPlayerCreateGame.Click += (o, e) => 
                CreateMultiplayerGame();
            
            panel.JoinByIpJoin.Click += (o, e) => 
                JoinServer(panel.IpAddressTextBox.Text);
            
            panel.GameLogo.Renderable = new TextureRegion(YetiGame.ContentManager.Load<Texture2D>("MainMenu/logo"));
            
            UserInterface.SetActive();
        }

        public override void Begin()
        {
            
        }

        public override void Finish()
        {
            
        }

        public override void Update()
        {
            base.Update();
            _networkManager.Update();
        }

        public override void RenderScreen()
        {
            
        }
        
        private async void StartNewGame()
        {
            var gameScreen = 
                ScreenManager.AddScreen(new GameScreen(_networkManager));

            var world = await CreateWorld();
            
            gameScreen.SetWorld(world);

            gameScreen.AddPlayer(new Player
            {
                HeadId = 1,
                BodyId = 1,
                LegsId = 1
            }, true);


            ScreenManager.ChangeScreen(gameScreen.Name);
        }
        
        private async Task<IGameWorld> CreateWorld()
        {
            _worldMaker = new WorldMaker()
                .SetWorldSize(200, 200);
            
            return await _worldMaker.Create(this);
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
        
        private void OnConnectedToLobby(Lobby obj)
        {
            var ip = obj.GetData("ip");
            JoinServer(ip.Trim());
        }
        
        private void CreateMultiplayerGame()
        {
            _networkManager.CreateSession();
            StartNewGame();
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
                
            ScreenManager.AddScreen(gameScreen);
            ScreenManager.ChangeScreen(gameScreen.Name);
        }
        
        private void ConnectToLobby(string lobbyId)
        {
            SteamMatchmaking.OnLobbyEntered += OnConnectedToLobby;
            SteamMatchmaking.JoinLobbyAsync(ulong.Parse(lobbyId));
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

        public void OnNewConnection(Connection connection, ConnectionInfo info)
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

        public void OnConnectedToServer(ConnectionInfo info) => 
            CreateNetworkWorld();

        public void OnWorldGenerationUpdate(string message)
        {
            
        }
        
    }
}