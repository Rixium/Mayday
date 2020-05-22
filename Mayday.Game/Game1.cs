using Mayday.Game.Screens;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Steamworks;

namespace Mayday.Game
{
    public class Game1 : YetiGame
    {

        public Game1() : base("Mayday")
        {
#if !DEBUG
            var steam = new Steam(1323490);
            steam.Exit += Exit;
            
            Name = steam.GetSteamName();
            FriendCount = steam.GetSteamFriendCount();
#endif
        }

        public static int FriendCount { get; set; }

        public static string Name { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F2), () => Yetiface.Engine.Utils.Window.ResizeWindow(640, 360));
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F3), () => Yetiface.Engine.Utils.Window.ResizeWindow(1280, 720));
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F4), () => Yetiface.Engine.Utils.Window.ResizeWindow(1920, 1080));
            
#if !DEBUG
            var music  = YetiGame.ContentManager.Load<Song>("MainMenu/menuMusic");
            MediaPlayer.Play(music);
#endif
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            
            ScreenManager.AddScreen(new MenuScreen());
            ScreenManager.AddScreen(new GameScreen());
        }
        
    }
    
}
