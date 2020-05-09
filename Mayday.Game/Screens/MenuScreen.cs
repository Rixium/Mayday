using Mayday.Game.UI;
using Mayday.Game.UI.Widgets;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    public class MenuScreen : IScreen
    {
        public IScreenManager ScreenManager { get; set; }
        
        public IUserInterface UserInterface { get; set; }
        public Color BackgroundColor { get; set; } = Color.Green;
        public string Name { get; set; } = "MenuScreen";

        public void Awake()
        {
            UserInterface =  new UserInterface();
            UserInterface.AddElement(new Panel());
        }

        public void Begin()
        {
            
        }
        
        public void Finish()
        {
            
        }
        public void Update()
        {
            
        }

        public void Draw()
        {
            
        }
        
    }
}