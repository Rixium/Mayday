using Myra.Graphics2D.UI;

namespace Yetiface.Engine.UI
{
    public class MyraUserInterface : IUserInterface
    {
        private readonly Panel _panel;
        public Desktop Desktop { get; set; }

        public MyraUserInterface(Panel panel)
        {
            _panel = panel;
            Desktop = new Desktop()
            {
                Root = panel
            };
        }

        public void Draw()
        {
            Desktop.Render();
        }

        public void Update()
        {
            //Desktop.Update();
            MouseOver = Desktop.IsMouseOverGUI;
        }

        public void AfterDraw()
        {
            
        }

        public bool MouseOver { get; set; }
        
    }
}