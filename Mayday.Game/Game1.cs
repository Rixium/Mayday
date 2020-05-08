using System.IO;
using Mayday.Game.Inputs;
using Mayday.Game.Screens;
using Mayday.Game.Screens.Transitions;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mayday.Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public string GameVersion => $"{major}.{_minor}.{_revision}";
        public string GameName = "Mayday";
        public string GameSubtitle = "The game that Mathias and Dan will finish";

        public static ContentManager ContentManager;
        public static IInputManager InputManager;

        /// <summary>
        /// Just do version numbers here.
        /// Don't need to change any of these until we have an actual build to release, but it's here
        /// because I want to look official.
        /// </summary>
        private int major = 0;

        private int _minor = 0;
        private int _revision = 0;

        private readonly IScreenManager _screenManager;

        public Game1()
        {
            Utils.Window.GraphicsDeviceManager = Utils.Window.CreateGraphicsDevice(this);

            Content.RootDirectory = "Content";
            _screenManager = new ScreenManager();
            ContentManager = Content;
        }

        protected override void Initialize()
        {
            SetupUtils();
            Window.Title = $"{GameName} - {GameSubtitle}";
            
            InputManager = new InputManager();
            InputManager.Initialize(File.ReadAllText("Config/inputBindings.json"));
            
            base.Initialize();
        }

        private void SetupUtils()
        {
            Window.ClientSizeChanged += (obj, eventArgs) => Utils.Window.WindowResized(Window);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            // Monogame stuffs !DO FIRST ALWAYS
            GraphicsUtils.SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO Load content in each window or load all the same time, what you think?

            _screenManager.SetScreenTransition(
                new FadeTransition
                {
                    Speed = 2f
                }
            );
            
            _screenManager.AddScreen(
                new SplashScreen
                {
                    BackgroundColor = Color.White
                }
             );
            
            _screenManager.AddScreen(new MenuScreen());
        }

        protected override void Update(GameTime gameTime)
        {
            // Update all of our util stuff !DO FIRST ALWAYS
            UtilManager.Update(gameTime);
            
            InputManager.Update();
            _screenManager.Update();
            
            // base update. Calls the base classes update method !DO LAST ALWAYS
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            _screenManager.Draw();
            base.Draw(gameTime);
        }

    }
}