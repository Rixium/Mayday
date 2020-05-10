using Mayday.Game.Screens;
using Yetiface.Engine;
using Yetiface.Steamworks;

namespace Mayday.Game
{
    public class Game1 : YetiGame
    {

        public Game1() : base("Mayday")
        {
            var steam = new Steam(0);
            steam.Exit += Exit;
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            
            ScreenManager.AddScreen(new MenuScreen());
        }
        
    }
    
}
