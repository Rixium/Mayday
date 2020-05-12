using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Screens
{
    internal class SplashScreen : Screen
    {
        public SplashScreen() : base("Splash")
        {
        }

        public override void Awake()
        {
            IsForced = false;

            var texture = YetiGame.ContentManager.Load<Texture2D>("Splash/splash");
            var sprite = new Sprite(texture);
            var splashEntity = CreateEntity(Window.Center);
            splashEntity.AddComponent(new SpriteRenderComponent(sprite));
        }

        public override void Begin() => ScreenManager.NextScreen();

        public override void Finish()
        {
        }
    }
}