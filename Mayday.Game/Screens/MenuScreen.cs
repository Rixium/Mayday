using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

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
            var ball = CreateEntity(Window.BottomRight - new Vector2(50, 50));
            ball.AddComponent(new Animation(YetiGame.ContentManager.Load<Texture2D>("Ball"),
                "Content/Assets/Ball.json"));

            ball.Scale = 3;
            
            var panel = UserInterface.SetRoot(new Panel()
            {
                Anchor = Anchor.Center
            });

            var image = YetiGame.ContentManager.Load<Texture2D>("MainMenu/menuImage");
            var imageElement = panel.AddElement(
                new Image(image, DrawMode.Stretch)
                {
                    Anchor = Anchor.Center
                }
            );
            
            var titlePanel = imageElement.AddElement(new Panel
            {
                FillColor = Color.Black * 0.5f,
                FillToParent = false,
                Height = 100,
                Width = 1280
            });
            
            titlePanel.AddElement(new TextBlock("Mayday"));

                                    
            var buttonPanel = panel.AddElement(new Panel
            {
                Offset = new Vector2(0, 720),
                Width = 1280,
                Height = 500,
                FillColor = Color.Black,
                FillToParent = false,
                Anchor = Anchor.Left
            });

            var playButton = buttonPanel.AddElement(new Button()
            {
                Anchor = Anchor.Left,
                FillColor = Color.Black,
                Height = 50,
                Width = 300,
                FillToParent = false
            });
            
            // playButton.AddElement(new TextBlock("Start Game"));
            //
            // var settingsButton = buttonPanel.AddElement(new Button()
            // {
            //     Anchor = Anchor.Left,
            //     FillColor = Color.Black,
            //     Height = 50,
            //     Width = 300,
            //     FillToParent = false
            // });
            //
            // settingsButton.AddElement(new TextBlock("Settings"));
            //
            // var quitButton = buttonPanel.AddElement(new Button()
            // {
            //     Anchor = Anchor.Left,
            //     FillColor = Color.Black,
            //     Height = 50,
            //     Width = 300,
            //     FillToParent = false
            // });
            //
            // quitButton.AddElement(new TextBlock("Quit"));
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }
    }
}