using Mayday.Game.Screens;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
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
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F2), () => Yetiface.Engine.Utils.Window.ResizeWindow(640, 360));
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F3), () => Yetiface.Engine.Utils.Window.ResizeWindow(1280, 720));
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F4), () => Yetiface.Engine.Utils.Window.ResizeWindow(1920, 1080));
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            
            ScreenManager.AddScreen(new MenuScreen());
        }
        
    }
    
}
