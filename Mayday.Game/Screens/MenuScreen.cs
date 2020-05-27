using Mayday.Game.Networking.SteamNetworking;
using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Screens;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;
        }

        public override void Awake()
        {
            var networkManager = new SteamNetworkManager(Game1.AppId);
            var menuScreenUserInterface = new MenuScreenUserInterface(networkManager, ScreenManager);
            UserInterface = menuScreenUserInterface;
        }

        public override void Begin()
        {
            
        }

        public override void Finish()
        {
            
        }
        
    }
}