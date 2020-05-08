using Mayday.Game.Screens;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        
        // TEMP FOR DEBUG
        private Texture2D _pixel;
        private SpriteFont _debugFont;
        private bool _isDebug;
        private KeyboardState _lastKeyState;
        
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

            // TODO Load content in each window or load all the same time, what you think?

            LoadDebug();
        }

        protected override void Update(GameTime gameTime)
        {
            // Update all of our util stuff !DO FIRST ALWAYS
            UtilManager.Update(gameTime);

            _screenManager.Update();
            
            UpdateDebug();
            
            // base update. Calls the base classes update method !DO LAST ALWAYS
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); 
            
            DrawDebug();

            _screenManager.Draw();

            base.Draw(gameTime);
        }
        
        // TODO Need to create dedicated debugger, rather than this shit mess.
        private void LoadDebug()
        {
            _pixel = Content.Load<Texture2D>("Utils/pixel");
            _debugFont = Content.Load<SpriteFont>("Fonts/debugFont");
        }

        private void UpdateDebug()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.OemTilde) && _lastKeyState.IsKeyUp(Keys.OemTilde)) 
                _isDebug = !_isDebug;

            _lastKeyState = keyState;

            if (_isDebug) return;
        }
        
        private void DrawDebug()
        {
            if (!_isDebug) return;
            
            Graphics.SpriteBatch.Begin();
            
            // WINDOW MEASUREMENTS
            Graphics.SpriteBatch.Draw(_pixel, new Rectangle(0, 10, Utils.Window.WindowWidth, 1), Color.White);
            Graphics.SpriteBatch.DrawString(_debugFont, "" + Utils.Window.WindowWidth, new Vector2(Utils.Window.WindowWidth / 2.0f, 30), Color.White);
            Graphics.SpriteBatch.Draw(_pixel, new Rectangle(10, 0, 1, Utils.Window.WindowHeight), Color.White);
            Graphics.SpriteBatch.DrawString(_debugFont, "" + Utils.Window.WindowHeight, new Vector2(30, Utils.Window.WindowHeight / 2.0f), Color.White);
            
            Graphics.SpriteBatch.DrawString(_debugFont, "Window Measurements", 
                new Vector2(40, 
                    40), Color.White);
            
            // VIEWPORT MEASUREMENTS
            Graphics.SpriteBatch.Draw(_pixel, new Rectangle(0, Utils.Window.WindowHeight - 10, Utils.Window.WindowWidth, 1), Color.Green);
            Graphics.SpriteBatch.DrawString(_debugFont, "" + Utils.Window.ViewportWidth, new Vector2(Utils.Window.WindowWidth / 2.0f, Utils.Window.WindowHeight - 10 - 30), Color.Green);
            Graphics.SpriteBatch.Draw(_pixel, new Rectangle(Utils.Window.WindowWidth - 10, 0, 1, Utils.Window.WindowHeight), Color.Green);
            Graphics.SpriteBatch.DrawString(_debugFont, "" + Utils.Window.ViewportHeight, new Vector2(Utils.Window.WindowWidth - 10 - 30, Utils.Window.WindowHeight / 2.0f), Color.Green);
            
            Graphics.SpriteBatch.DrawString(_debugFont, "Viewport Measurements", 
                new Vector2(Utils.Window.WindowWidth - _debugFont.MeasureString("Viewport Measurements").X - 40, 
                    Utils.Window.WindowHeight - _debugFont.MeasureString("Viewport Measurements").Y - 40), Color.Green);
            
            Graphics.SpriteBatch.End();
        }
        
    }
}
