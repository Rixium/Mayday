using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Screens.Transitions;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class YetiGame : Game
    {
       public string GameVersion => $"{Major}.{Minor}.{Revision}";
        public string GameName = "YetiGame";
        
        public static ContentManager ContentManager;
        public static IInputManager InputManager;
        public IScreenManager ScreenManager { get; set; }

        /// <summary>
        /// Just do version numbers here.
        /// Don't need to change any of these until we have an actual build to release, but it's here
        /// because I want to look official.
        /// </summary>
        protected int Major = 0;

        protected int Minor = 0;
        protected int Revision = 0;


        public YetiGame(string gameName)
        {
            GameName = gameName;
            
            Utils.Window.GraphicsDeviceManager = Utils.Window.CreateGraphicsDevice(this);

            Content.RootDirectory = "Content";
            ScreenManager = new ScreenManager();
            ContentManager = Content;
        }

        protected override void Initialize()
        {
            SetupUtils();
            Window.Title = $"{GameName} - {GameVersion}";
            
            InputManager = new InputManager();
            InputManager.Initialize(File.ReadAllText("Config/inputBindings.json"));

            base.Initialize();
        }

        private void SetupUtils()
        {
            Window.ClientSizeChanged += (obj, eventArgs) => Utils.Window.WindowResized(Window);
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            GraphicsUtils.Instance.SpriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsUtils.Instance.Load(ContentManager);
            
            ScreenManager.SetScreenTransition(
                new FadeTransition
                {
                    Speed = 3f
                }
            );
            
            ScreenManager.AddScreen(
                new SplashScreen
                {
                    BackgroundColor = Color.White
                }
             );
        }

        protected override void Update(GameTime gameTime)
        {
            // Update all of our util stuff !DO FIRST ALWAYS
            UtilManager.Update(gameTime);
            
            InputManager.Update();
            ScreenManager.Update();
            
            // base update. Calls the base classes update method !DO LAST ALWAYS
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            ScreenManager.Draw();
            base.Draw(gameTime);
        }
        
    }
}
