using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Screens
{
    public class UiTestScreen : Screen
    {
        public UiTestScreen() : base("UiTest")
        {
        }

        public override void Awake()
        {
            var buttonPanel = UserInterface.AddElement(new Panel
            {
                FillColor = Color.Blue * 0.5f,
                Anchor = Anchor.Center,
                Size = new Vector2(0.5f, 0.5f)
            });
            
            UserInterface.AddElement(new Button("CenterLeft")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.CenterLeft
            });
            
            UserInterface.AddElement(new Button("CenterRightDwight")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.CenterRight
            });
            
            buttonPanel.AddElement(new Button("Center")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.Center
            });
             
            buttonPanel.AddElement(new Button("CenterLeft")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.CenterLeft
            });
 
            buttonPanel.AddElement(new Button("CenterRight")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.CenterRight
            });

            buttonPanel.AddElement(new Button("TopLeft")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.TopLeft
            });
            
            buttonPanel.AddElement(new Button("TopCenter")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.TopCenter
            });
            
            buttonPanel.AddElement(new Button("TopRight")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.TopRight
            });
            
            buttonPanel.AddElement(new Button("BottomRight")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.BottomRight
            });
            
            buttonPanel.AddElement(new Button("BottomLeft")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.BottomLeft
            });
            
            buttonPanel.AddElement(new Button("BottomCenter")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.BottomCenter
            });

            UserInterface.AddElement(new Button("TopRight")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.TopRight
            });
            
            UserInterface.AddElement(new Button("TopLeft")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.TopLeft
            });

            UserInterface.AddElement(new Button("TopCenter")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.TopCenter
            });

            var buttonBottomLeft = UserInterface.AddElement(new Button("BottomLeft")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.BottomLeft
            });
            
            UserInterface.AddElement(new Button("BottomCenter")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.BottomCenter
            });
            
            UserInterface.AddElement(new Button("BottomRight")
            {
                Size = new Vector2(0.15f, 50),
                Anchor = Anchor.BottomRight
            });
            
            buttonBottomLeft.OnClicked += OnButtonClicked;
            buttonBottomLeft.OnHover += () =>
            {
                buttonBottomLeft.FillColor = Color.Pink;
            };
            buttonBottomLeft.OnLeave += () =>
            {
                buttonBottomLeft.FillColor = Color.Black;
            };
            
        }

        private void OnButtonClicked()
        {
            ScreenManager.ChangeScreen("MenuScreen");
        }

        public override void Begin()
        {
            
        }

        public override void Finish()
        {
            
        }
    }
}