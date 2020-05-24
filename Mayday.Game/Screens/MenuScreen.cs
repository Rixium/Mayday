using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;
using Yetiface.Engine.Screens;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        private INetworkManager _networkManager;
        
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;
            var menuScreenUserInterface = new MenuScreenUserInterface();
            UserInterface = menuScreenUserInterface;
            
            _networkManager = new SteamNetworkManager(Game1.AppId);

            menuScreenUserInterface.HostGameClicked += HostGame;
            _networkManager.OnUserJoined += OnUserJoined;
        }

        private int _connectedUsers = 1;
        
        private void OnUserJoined()
        {
            _connectedUsers++;
            ShowServer();
        }

        private void HostGame()
        {
            _networkManager.CreateSession(OnLobbyCreated);
        }

        private void OnLobbyCreated() => ShowServer();

        public void ShowServer()
        {
            (UserInterface as MenuScreenUserInterface)?.ShowServer(_connectedUsers);
        }
        
        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }
    }
}