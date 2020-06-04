using Mayday.Game.Networking.SteamNetworking;
using Mayday.Game.UI;
using Mayday.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;

            var panel = new MainMenuUserInterface();
            UserInterface = new MyraUserInterface(panel);
        }

        public override void Awake()
        {
            var networkManager = new SteamNetworkManager(Game1.AppId);
            UserInterface.SetActive();
        }

        public override void Begin()
        {
            
        }

        public override void Finish()
        {
            
        }

        public override void Draw()
        {
            base.Draw();
            
            Desktop.Render();
        }
    }
}