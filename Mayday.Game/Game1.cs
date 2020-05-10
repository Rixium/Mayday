using Mayday.Game.Screens;
using Yetiface.Engine;

namespace Mayday.Game
{
    public class Game1 : YetiGame
    {

        public Game1() : base("Mayday")
        {
            
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            
            ScreenManager.AddScreen(new MenuScreen());
        }
        
    }
    
}
