using System;
using Mayday.Game.ECS;
using Mayday.Game.ECS.Components.Renderables;
using Mayday.Game.Graphics;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public class SplashScreen : Screen
    {
        private bool _isReady;

        private float _spentTime;
        private float _transValue;
        private const float StayTime = 5f; //seconds

        private IEntity _logoEntity;

        private bool _shouldRotate;
        private float _angleIncreaseExp;

        private SpriteRenderComponent _logoSpriteComponent;

        public SplashScreen() : base("Splash")
        {
        }

        public override void Awake()
        {
            var logoSprite = new Sprite(Game1.ContentManager.Load<Texture2D>("Splash/splash"));
            _logoEntity = CreateEntity("logo");
            _logoEntity.Position = Window.Center;
            _logoSpriteComponent = _logoEntity.AddComponent(new SpriteRenderComponent(logoSprite));

            var ballImage = Game1.ContentManager.Load<Texture2D>("Ball");
            var ballEntity = CreateEntity("ball");
            ballEntity.Position = Window.BottomRight + new Vector2(-50, -50);
            ballEntity.Scale = 3;
            
            var animationComponent = new Animation(ballImage, "Content/Assets/Ball.json");
            ballEntity.AddComponent(animationComponent);
            
            Game1.InputManager.RegisterInputEvent("interact", OnInteractPressed);
            Game1.InputManager.RegisterInputEvent("secret", OnRotatePressed);
        }

        private void OnInteractPressed()
        {
            _spentTime = StayTime;
            _isReady = true;
        }

        private void OnRotatePressed()
        {
            _isReady = true;
            _shouldRotate = true;
        }

        public override void Begin()
        {
            _isReady = true;
        }

        public override void Update()
        {
            if (!_isReady) return;

            if (_shouldRotate)
            {
                _logoEntity.Rotation += 0.01f * _angleIncreaseExp;
                _angleIncreaseExp += 0.05f;
                _logoEntity.Scale -= 0.002f;
            }

            _spentTime += Time.DeltaTime;

            if (_spentTime <= StayTime)
            {
                _transValue += Time.DeltaTime;
            }
            else
            {
                _transValue -= Time.DeltaTime;
            }

            _transValue = MathHelper.Clamp(_transValue, 0, 1);

            if (_spentTime > StayTime + 1.5f && Math.Abs(_transValue) < 0.001f)
            {
                ScreenManager.ChangeScreen("MenuScreen");
            }

            _logoSpriteComponent.Color = Color.White * _transValue;

            base.Update();
        }

        public override void Finish()
        {
            // Removing our input events, because if we don't
            // they'll stay in the list on the next screen and will still be called BAD!
            Game1.InputManager.DeRegisterInputEvent("interact", OnInteractPressed);
            Game1.InputManager.DeRegisterInputEvent("secret", OnRotatePressed);
        }
    }
}