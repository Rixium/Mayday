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
        }

        private void HostGame() => _networkManager.CreateSession(OnLobbyCreated);

        private void OnLobbyCreated(MyServer server) => ShowServer(server);

        public void ShowServer(MyServer server) => (UserInterface as MenuScreenUserInterface)?.ShowServer(_networkManager, server);

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