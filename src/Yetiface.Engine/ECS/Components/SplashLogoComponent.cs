using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.ECS.Components
{
    internal class SplashLogoComponent : Component
    {
        private readonly IScreenManager _screenManager;

        private float _spentTime;
        private const float StayTime = 5f; //seconds

        public SplashLogoComponent(IScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        public override void Update()
        {
            _spentTime += Time.DeltaTime;

            if (_spentTime < StayTime + 1.5f) return;

            _screenManager.NextScreen();
        }

        public override void Draw()
        {
            
        }
    }
}