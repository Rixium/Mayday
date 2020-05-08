using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public class SplashScreen : IScreen
    {
        public string Name { get; set; } = "Splash";
        private Texture2D _image;

        public SplashScreen()
        {
            _image = Game1.ContentManager.Load<Texture2D>("Splash/splash");
        }
        public void Update()
        {
            
        }

        public void Draw()
        {
            Graphics.SpriteBatch.Draw(_image, new Vector2(0,0), Color.White);
        }
    }
}