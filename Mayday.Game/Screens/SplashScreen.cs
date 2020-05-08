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
        private Vector2 _spritePos;

        public SplashScreen()
        {
            var image = Game1.ContentManager.Load<Texture2D>("Splash/splash");
            _sprite = new Sprite(image);
            _spritePos = Window.Center;
        }
        
        public void Update()
        {
            
        }

        public void Draw()
        {
            GraphicsUtils.Draw(_sprite, _spritePos);
        }
        
    }
}