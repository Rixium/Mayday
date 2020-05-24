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

        public static Steam Steam { get; set; }
        
        public Game1() : base("Mayday")
        {
            Steam = new Steam(1323490);
            Steam.Exit += Exit;
            
            Name = Steam.GetSteamName();
            FriendCount = Steam.GetSteamFriendCount();
        }

        public static int FriendCount { get; set; }

        public static string Name { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F1), NextResize);

            var music  = ContentManager.Load<Song>("MainMenu/menuMusic");
            MediaPlayer.Play(music);
            MediaPlayer.Volume = 0.1f;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            
            ScreenManager.AddScreen(new MenuScreen());
            ScreenManager.AddScreen(new GameScreen());
        }

        public static void NextResize()
        {
            switch (Yetiface.Engine.Utils.Window.WindowWidth)
            {
                case 640:
                    Yetiface.Engine.Utils.Window.ResizeWindow(1280, 720);
                    break;
                case 1280:
                    Yetiface.Engine.Utils.Window.ResizeWindow(1920, 1080);
                    break;
                case 1920:
                    Yetiface.Engine.Utils.Window.ResizeWindow(640, 360);
                    break;
            }
        }
    }
    
}
