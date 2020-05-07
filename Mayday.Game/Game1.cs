using Mayday.Game.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mayday.Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly IScreenManager _screenManager;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            _screenManager = new ScreenManager();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            base.Draw(gameTime);
        }
        
        
    }
}
