using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.ECS.Components
{
    internal class SplashLogoComponent : Component
    {
        private readonly IScreenManager _screenManager;

        private float _spentTime;
        private const float StayTime = 5f; //seconds

        private bool _shouldRotate;
        private float _angleIncreaseExp;

        public SplashLogoComponent(IScreenManager screenManager)
        {
            _screenManager = screenManager;
            YetiGame.InputManager.RegisterInputEvent("secret", OnRotatePressed);
        }

        private void OnRotatePressed()
        {
            _shouldRotate = true;
        }

        public override void Update()
        {
            if (_shouldRotate)
            {
                Entity.Rotation += 0.01f * _angleIncreaseExp;
                _angleIncreaseExp += 0.05f;
                Entity.Scale -= 0.002f;
            }

            _spentTime += Time.DeltaTime;

            if (_spentTime < StayTime + 1.5f) return;

            _screenManager.NextScreen();
        }

        public override void Draw()
        {
            
        }
    }
}