using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
        }

        public override void Awake()
        {
            var ball = CreateEntity(Window.BottomRight - new Vector2(50, 50));
            ball.AddComponent(new Animation(YetiGame.ContentManager.Load<Texture2D>("Ball"),
                "Content/Assets/Ball.json"));

            ball.Scale = 3;
            
            BackgroundColor = Color.Green;

            var panel = UserInterface.SetRoot(new Panel());

            panel.AddElement(new Panel(50, 50));
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }
    }
}