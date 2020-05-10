using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
            var ballImage = YetiGame.ContentManager.Load<Texture2D>("Ball");
            var ballEntity = CreateEntity("ball");
            ballEntity.Position = Window.BottomRight + new Vector2(-50, -50);
            ballEntity.Scale = 3;
            
            var animationComponent = new Animation(ballImage, "Content/Assets/Ball.json");
            ballEntity.AddComponent(animationComponent);
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
        
    }
}