using Mayday.Game.UI.Widgets;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
        }

        public override void Awake()
        {
            BackgroundColor = Color.Green;

            UserInterface.AddElement(new Panel
            {
                X = 0,
                Y = 0,
                Width = Window.ViewportWidth,
                Height = Window.ViewportHeight
            });
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
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}