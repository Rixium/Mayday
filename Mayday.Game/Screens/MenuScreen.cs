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
            var image = YetiGame.ContentManager.Load<Texture2D>("MainMenu/planet");
            
            _imageElement = UserInterface.AddElement(
                new Image(image)
                {
                    Size = new Vector2(0.5f, 0.5f),
                    Anchor = Anchor.Center
                }
            );

            var titlePanel = UserInterface.AddElement(new Panel()
            {
                FillColor = Color.Black * 0.5f,
                Anchor = Anchor.TopCenter,
                Size = new Vector2(1, 0.2f)
            });
            
            titlePanel.AddElement(new TextBlock("Mayday"));

            var buttonPanel = UserInterface.AddElement(new Panel
            {
                Anchor = Anchor.Center,
                Size = new Vector2(0.3f, 0.2f)
            });

            var playButton = buttonPanel.AddElement(new Button("Start Game")
            {
                Anchor = Anchor.TopCenter,
                Size = new Vector2(1, 60),
                FillColor = Color.Black * 0.5f,
            });
            
            var settingsButton = buttonPanel.AddElement(new Button("Settings")
            {
                Anchor = Anchor.TopCenter,
                Size = new Vector2(1, 60),
                Offset = new Vector2(0, 70),
                FillColor = Color.Black * 0.5f,
            });
            
            var exitButton = buttonPanel.AddElement(new Button("Exit")
            {
                Anchor = Anchor.TopCenter,
                Size = new Vector2(1, 60),
                Offset = new Vector2(0, 140),
                FillColor = Color.Black * 0.5f,
            });

            playButton.OnHover += OnButtonHover;
            playButton.OnLeave += OnHoverLeave;
            settingsButton.OnHover += OnButtonHover;
            settingsButton.OnLeave += OnHoverLeave;
            exitButton.OnHover += OnButtonHover;
            exitButton.OnLeave += OnHoverLeave;
        }

        private void OnHoverLeave(IElement obj)
        {
            obj.FillColor = Color.Black * 0.5f;
        }

        private void OnButtonHover(IElement obj)
        {
            obj.FillColor = Color.Black * 0.8f;
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