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
                Anchor = Anchor.CenterLeft
            });

            UserInterface.AddElement(new Button("TopLeft"){Anchor = Anchor.TopLeft});
            UserInterface.AddElement(new Button("TopCenter") {Anchor = Anchor.TopCenter});
            UserInterface.AddElement(new Button("TopRight") {Anchor = Anchor.TopRight});
            
            UserInterface.AddElement(new Button("CenterLeft") {Anchor = Anchor.CenterLeft});
            UserInterface.AddElement(new Button("CenterRightDwight") {Anchor = Anchor.CenterRight});
            UserInterface.AddElement(new Button("Center") {Anchor = Anchor.Center});
            
            var buttonBottomLeft = UserInterface.AddElement(new Button("BottomLeft") {Anchor = Anchor.BottomLeft});
            UserInterface.AddElement(new Button("BottomLeftNext") {Anchor = Anchor.BottomLeft});
            UserInterface.AddElement(new Button("BottomCenter") {Anchor = Anchor.BottomCenter});
            UserInterface.AddElement(new Button("BottomRight") {Anchor = Anchor.BottomRight});

            buttonBottomLeft.OnClicked += OnButtonClicked;
            buttonBottomLeft.OnHover += () =>
            {
                buttonBottomLeft.FillColor = Color.Pink;
            };
            buttonBottomLeft.OnLeave += () =>
            {
                buttonBottomLeft.FillColor = Color.Black;
            };

            var topPanel = UserInterface.AddElement(new Panel(){FillColor = Color.Aqua*0.7f, Width = Window.WindowWidth, Height = 70, Anchor = Anchor.TopCenter});
            topPanel.AddElement(new Button("Am top :D") {Anchor = Anchor.TopLeft});
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