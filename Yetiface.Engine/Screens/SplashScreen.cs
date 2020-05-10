using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.ECS.Components;
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
            
            var logoSprite = new Sprite(YetiGame.ContentManager.Load<Texture2D>("Splash/splash"));
            var logoEntity = CreateEntity("logo");
            logoEntity.Position = Window.Center;
            logoEntity.AddComponent(new SpriteRenderComponent(logoSprite));
            logoEntity.AddComponent(new SplashLogoComponent(ScreenManager));
        }

        public override void Begin()
        {

        }

        public override void Finish()
        {

        }
    }
}