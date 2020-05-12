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

            var panel = UserInterface.AddElement(new Panel
            {
                X = 0,
                Y = 0,
                Width = Window.ViewportWidth,
                Height = Window.ViewportHeight
            });

            panel.AddElement(new Panel()
            {
                X = 10,
                Y = 10,
                Width = 100,
                Height = 100
            });
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }
    }
}