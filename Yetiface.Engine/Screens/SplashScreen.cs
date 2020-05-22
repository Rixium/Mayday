using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Screens
{
    internal class SplashScreen : Screen
    {
        private Sprite _sprite;

        public SplashScreen() : base("Splash")
        {
        }

        public override void Awake()
        {
            IsForced = false;

            var texture = YetiGame.ContentManager.Load<Texture2D>("Splash/splash");
            _sprite = new Sprite(texture);
        }

        public override void Begin() => ScreenManager.NextScreen();

        public override void Finish()
        {
        }

        public override void Draw()
        {
            base.Draw();

            GraphicsUtils.Instance.Begin(false);
            GraphicsUtils.Instance.Draw(_sprite, new Vector2(Window.WindowWidth / 2.0f, Window.WindowHeight / 2.0f), Color.White);
            GraphicsUtils.Instance.End();
        }
    }
}