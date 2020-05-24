using GeonBit.UI;
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

            UserInterface = new MenuScreenUserInterface();
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