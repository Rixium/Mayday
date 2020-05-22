using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Yetiface.Engine;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        private Image _imageElement;

        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;
        }

        public override void Awake()
        {
            var panel = UserInterface.SetRoot(new Panel()
            {
                Anchor = Anchor.Center,
                FillToParent = true
            });

            var image = YetiGame.ContentManager.Load<Texture2D>("MainMenu/planet");
            _imageElement = panel.AddElement(
                new Image(image, fillToParent: false)
                {
                    Offset = new Vector2(0,100),
                    Anchor = Anchor.Center
                }
            );

            var titlePanel = panel.AddElement(new Panel()
            {
                FillToParent = false,
                Width = 10000,
                Height = 80,
                FillColor = Color.Black * 0.5f
            });
            
            titlePanel.AddElement(new TextBlock("Mayday"));

            var buttonPanel = panel.AddElement(new Panel(0, 400)
            {
                Anchor = Anchor.Auto,
                FillToParent = false,
                Width = 500,
                Height = 300,
                FillColor = Color.Black * 0
            });

            var playButton = buttonPanel.AddElement(new Button()
            {
                FillToParent = false,
                Width = 500,
                Height = 100,
                FillColor = Color.Black * 0f
            });
            
            playButton.AddElement(new TextBlock("Start"));
            
            var settingsButton = buttonPanel.AddElement(new Button()
            {
                FillToParent = false,
                Width = 500,
                Height = 100,
                FillColor = Color.Black * 0f
            });
            settingsButton.AddElement(new TextBlock("Settings"));
            
            var exitButton = buttonPanel.AddElement(new Button()
            {
                FillToParent = false,
                Width = 500,
                Height = 100,
                FillColor = Color.Black * 0f
            });
            
            exitButton.AddElement(new TextBlock("Exit"));
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

            _imageElement.Rotation += Time.DeltaTime * 0.05f;
        }
    }
}