using Mayday.Game.Screens;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public string GameVersion => $"{major}.{_minor}.{_revision}";
        public string GameName = "Mayday";
        public string GameSubtitle = "The game that Mathias and Dan will finish";

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
        }
        
        protected override void Initialize()
        {
            SetupUtils();
            Window.Title = $"{GameName} - {GameSubtitle}";
            base.Initialize();
        }
        
        private void SetupUtils()
        {
            Window.ClientSizeChanged += (obj, eventArgs) => Utils.Window.WindowResized(Window);

            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Monogame stuffs !DO FIRST ALWAYS
            Graphics.SpriteBatch  = new SpriteBatch(GraphicsDevice);
            
            // Loady Loady..
            // TODO Load content in each window or load all the same time, what you think?
        }

        protected override void Update(GameTime gameTime)
        {
            // Update all of our util stuff !DO FIRST ALWAYS
            UtilManager.Update(gameTime);
            
            _screenManager.Update();

            // base update. Calls the base classes update method !DO LAST ALWAYS
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            _screenManager.Draw();

            base.Draw(gameTime);
        }

    }
}
