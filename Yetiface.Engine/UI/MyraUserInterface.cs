using System;
using System.Linq;
using System.Reflection;
using Myra.Graphics2D.UI;

namespace Yetiface.Engine.UI
{
    public class MyraUserInterface : IUserInterface
    {
        private readonly Panel _panel;

        public MyraUserInterface(Panel panel)
        {
            _panel = panel;
            InitializeController(panel);
        }

        private static void InitializeController(Panel panel)
        {
            var assemblyTypes = panel.GetType().Assembly.GetTypes();
            var controller =
                assemblyTypes.FirstOrDefault(m => m.IsClass && m.Name.Equals(panel.GetType().Name + "Controller"));
            if (controller == null) return;
            Activator.CreateInstance(controller, panel);
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