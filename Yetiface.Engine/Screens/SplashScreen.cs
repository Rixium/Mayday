using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.ECS;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Screens
{
    internal class SplashScreen : Screen
    {
        private bool _isReady;

        private float _spentTime;
        private const float StayTime = 5f; //seconds

        private readonly IEntity _logoEntity;

        private bool _shouldRotate;
        private float _angleIncreaseExp;

        private SpriteRenderComponent _logoSpriteComponent;

        public SplashScreen() : base("Splash")
        {
            IsForced = true;

            var logoSprite = new Sprite(YetiGame.ContentManager.Load<Texture2D>("Splash/splash"));
            _logoEntity = CreateEntity("logo");
            _logoEntity.Position = Window.Center;
            _logoSpriteComponent = _logoEntity.AddComponent(new SpriteRenderComponent(logoSprite));
        }

        public override void Awake()
        {
            _isReady = false;
            _angleIncreaseExp = 0;
            _spentTime = 0;
            _shouldRotate = false;
            _logoEntity.Rotation = 0;
            _logoEntity.Scale = 1;

            YetiGame.InputManager.RegisterInputEvent("interact", OnInteractPressed);
            YetiGame.InputManager.RegisterInputEvent("secret", OnRotatePressed);
        }

        private void OnInteractPressed()
        {
            IsForced = false;
            ScreenManager.NextScreen();
        }

        private void OnRotatePressed() => _shouldRotate = true;

        public override void Begin() => _isReady = true;

        public override void Update()
        {
            base.Update();
            if (!_isReady) return;

            if (_shouldRotate)

            {
                _logoEntity.Rotation += 0.01f * _angleIncreaseExp;
                _angleIncreaseExp += 0.05f;
                _logoEntity.Scale -= 0.002f;
            }

            _spentTime += Time.DeltaTime;

            if (_spentTime < StayTime + 1.5f) return;

            IsForced = false;
            ScreenManager.NextScreen();
        }

        public override void Finish()
        {
            YetiGame.InputManager.DeRegisterInputEvent("interact", OnInteractPressed);
            YetiGame.InputManager.DeRegisterInputEvent("secret", OnRotatePressed);
        }
    }
}