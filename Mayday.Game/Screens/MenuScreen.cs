using GeonBit.UI;
using GeonBit.UI.Animators;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.Screens;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;
            
            UserInterface.Initialize(YetiGame.ContentManager);

            var panel = UserInterface.Active.AddEntity(new Panel(new Vector2(0, 0))
            {
                FillColor = Color.Black
            });

            panel.Padding = new Vector2(30, 30);

            var centerPanel = panel.AddChild(new Panel(new Vector2(0.5f, 0.5f), PanelSkin.None));
            
            var image = centerPanel.AddChild(new Image(YetiGame.ContentManager.Load<Texture2D>("MainMenu/planet"), new Vector2(0.8f, 0.8f), ImageDrawMode.Stretch, Anchor.Center)
            {
                EnforceSquare = true
            });
            
            image.SpaceBefore = new Vector2(100, 100);
            image.SpaceAfter =  new Vector2(100, 100);


            image.AttachAnimator(new RotationAnimator());
            
            var titlePanel = panel.AddChild(new Panel(new Vector2(0, 0.1f), PanelSkin.None, Anchor.TopCenter));
            var title = titlePanel.AddChild(new Header("Mayday", Anchor.Center)
            {
                FillColor = Color.White,
                OutlineColor = Color.Black
            });
            
            var buttonPanel = panel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center));
            var b1 = buttonPanel.AddChild(new Button("Single Player"));
            buttonPanel.AddChild(new Button("Multiplayer"));
            buttonPanel.AddChild(new Button("Settings"));
            buttonPanel.AddChild(new Button("Exit"));

            var bottomText = panel.AddChild(new RichParagraph("YetiFace", Anchor.BottomCenter)
            {
                Scale = 0.8f
            });
            
            bottomText.AttachAnimator(new TextWaveAnimator());

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