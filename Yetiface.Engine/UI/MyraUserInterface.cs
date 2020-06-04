using Myra.Graphics2D.UI;

namespace Yetiface.Engine.UI
{
    public class MyraUserInterface : IUserInterface
    {
        private readonly Panel _panel;

        public MyraUserInterface(Panel panel)
        {
            _panel = panel;
        }
        
        public void SetActive() => Desktop.Root = _panel;

        public void Draw()
        {
            Desktop.Render();
        }

        public void Update()
        {
            
        }

        public void AfterDraw()
        {
            
        }
    }
}