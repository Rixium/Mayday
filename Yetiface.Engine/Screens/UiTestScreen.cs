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
            var rootPanel = UserInterface.SetRoot(new Panel(){FillToParent = true});

            var buttonPanel = rootPanel.AddElement(new Panel(20){Height = (int) (rootPanel.Height * 0.7f), Width = 700, FillColor = Color.Blue*0.5f, FillToParent = false, Anchor = Anchor.CenterLeft});

            rootPanel.AddElement(new Button("TopLeft"){Anchor = Anchor.TopLeft});
            rootPanel.AddElement(new Button("TopCenter") {Anchor = Anchor.TopCenter});
            rootPanel.AddElement(new Button("TopRight") {Anchor = Anchor.TopRight});
            
            rootPanel.AddElement(new Button("CenterLeft") {Anchor = Anchor.CenterLeft});
            rootPanel.AddElement(new Button("Center") {Anchor = Anchor.Center});
            rootPanel.AddElement(new Button("CenterRight") {Anchor = Anchor.CenterRight});
            
            rootPanel.AddElement(new Button("BottomLeft") {Anchor = Anchor.BottomLeft});
            rootPanel.AddElement(new Button("BottomCenter") {Anchor = Anchor.BottomCenter});
            rootPanel.AddElement(new Button("BottomRight") {Anchor = Anchor.BottomRight});
            

            var topPanel = rootPanel.AddElement(new Panel(){FillColor = Color.Aqua*0.7f, Width = Window.WindowWidth, Height = 70, FillToParent = false, Anchor = Anchor.TopCenter});
            topPanel.AddElement(new Button("Am top :D") {Anchor = Anchor.TopLeft});
        }

        public override void Begin()
        {
            
        }

        public override void Finish()
        {
            
        }
    }
}