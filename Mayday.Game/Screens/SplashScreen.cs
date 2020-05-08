using Mayday.Game.Graphics;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public class SplashScreen : IScreen
    {
        public string Name { get; set; } = "Splash";
        
        public Color BackgroundColor { get; set; } = Color.White;
        
        private readonly Sprite _sprite;

        public SplashScreen()
        {
            var image = Game1.ContentManager.Load<Texture2D>("Splash/splash");
            _sprite = new Sprite(image);
        }
        
        public void Update()
        {
            
        }

        public void Draw()
        {
            GraphicsUtils.Draw(_sprite, Window.Center);
        }
        
    }
}