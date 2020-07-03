using System;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Networking.SteamNetworking;
using Mayday.Game.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Color = Microsoft.Xna.Framework.Color;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen, IWorldMakerListener, INetworkClientListener
    {
        private INetworkManager _networkManager;
        private IWorldMaker _worldMaker;
        
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = new Color(4,13,21);
        }

        public override void Awake()
        {
            _networkManager = new SteamNetworkManager(Game1.AppId, new MaydayMessagePackager());
            
            var panel = new MainMenuUserInterface();
            var controller = new MainMenuUserInterfaceController(panel);
            UserInterface = new MyraUserInterface(panel);
            
            _networkManager.SetClientNetworkListener(this);
            
            panel.StartGameNewGame.Click += (o, e) => 
                StartNewGame();
            
            panel.CreateMultiplayerGameCreateGame.Click += (o, e) => 
                CreateMultiplayerGame(panel.CreateMultiplayerGamePortTextBox.Text);
            
            panel.JoinByIpJoin.Click += (o, e) => 
                JoinServer(panel.IpAddressTextBox.Text, panel.PortTextBox.Text);
            
            panel.GameLogo.Renderable = new TextureRegion(YetiGame.ContentManager.Load<Texture2D>("MainMenu/logo"));
            
            panel.GameLogo.Renderable = new TextureRegion(YetiGame.ContentManager.Load<Texture2D>("MainMenu/logo"));
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
            var world = await CreateWorld();

            var gameScreen = 
                ScreenManager.AddScreen(
                    new GameScreen(world, _networkManager));

            gameScreen.AddPlayer(new Entity(SteamClient.SteamId), true);


            ScreenManager.ChangeScreen(gameScreen.Name);
        }
        
        private async Task<IGameWorld> CreateWorld()
        {
            _worldMaker = new WorldMaker()
                .SetWorldSize(1000,1000);
            
            return await _worldMaker.Create(this);
        }
        
        private void JoinServer(string ipAddress, string port)
        {
            try
            {
                _networkManager.JoinSession(ipAddress, port);
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

        private void CreateMultiplayerGame(string port)
        {
            _networkManager.CreateSession(port);
            StartNewGame();
        }
        
        private async void CreateNetworkWorld()
        { 
            _worldMaker = new NetworkWorldMaker(_networkManager);
            
            var world = await _worldMaker
                .Create(this);
            
            var gameScreen = new GameScreen(world, _networkManager);

            var player = gameScreen.AddPlayer(new Entity(SteamClient.SteamId), true);

            var newPlayerPacket = new NewPlayerPacket
            {
                SteamId = player.EntityId,
                HeadId = 1,
                X = (int) player.X,
                Y = (int) player.Y
            };

            var package = _networkManager.MessagePackager.Package(newPlayerPacket);
            _networkManager.SendMessage(package);

            foreach (var netPlayer in ((NetworkWorldMaker) _worldMaker).PlayersToAdd)
            {
                gameScreen.AddPlayer(new Entity(netPlayer.SteamId)
                {
                    X = netPlayer.X,
                    Y = netPlayer.Y
                });
            }
            
            ScreenManager.AddScreen(gameScreen);
            ScreenManager.ChangeScreen(gameScreen.Name);
        }
        
        // private void ConnectToLobby(string lobbyId)
        // {
        //     SteamMatchmaking.OnLobbyEntered += OnConnectedToLobby;
        //     SteamMatchmaking.JoinLobbyAsync(ulong.Parse(lobbyId));
        // }
        
        //         
        // private void OnConnectedToLobby(Lobby obj)
        // {
        //     var ip = obj.GetData("ip");
        //     var port = obj.GetData("port");
        //     JoinServer(ip.Trim(), port);
        // }
        
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

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            
        }

        public void OnConnectedToServer(ConnectionInfo info) => 
            CreateNetworkWorld();

        public void AddConsumer(IPacketConsumer packetConsumer)
        {
            
        }

        public void OnWorldGenerationUpdate(string message)
        {
            
        }
        
    }
}