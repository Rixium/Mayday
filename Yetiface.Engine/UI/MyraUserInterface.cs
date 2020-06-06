using Myra.Graphics2D.UI;

namespace Yetiface.Engine.UI
{
    public class MyraUserInterface : IUserInterface
    {
        private readonly Panel _panel;
        private Desktop _desktop;

        public MyraUserInterface(Panel panel)
        {
            _panel = panel;
            _desktop = new Desktop()
            {
                Root = panel
            };
        }

        public void Draw()
        {
            _desktop.Render();
        }

        public void Update()
        {
            
        }

        public void AfterDraw()
        {
            
        }
    }
}