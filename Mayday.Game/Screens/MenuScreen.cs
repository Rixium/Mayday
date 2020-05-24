using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Networking.SteamNetworking;
using Yetiface.Engine.Screens;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;

            var networkManager = new SteamNetworkManager(Game1.AppId);

            var menuScreenUserInterface = new MenuScreenUserInterface(networkManager);
            UserInterface = menuScreenUserInterface;
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