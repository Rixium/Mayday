using Mayday.Game.Screens;
using Yetiface.Engine;
using Yetiface.Steamworks;

namespace Mayday.Game
{
    public class Game1 : YetiGame
    {

        public Game1() : base("Mayday")
        {
            var steam = new Steam(1323490);
            steam.Exit += Exit;

            Name = steam.GetSteamName();
            FriendCount = steam.GetSteamFriendCount();
        }

        public static int FriendCount { get; set; }

        public static string Name { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = true; // TODO just for debugging.
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            
            ScreenManager.AddScreen(new MenuScreen());
        }
        
    }
    
}
