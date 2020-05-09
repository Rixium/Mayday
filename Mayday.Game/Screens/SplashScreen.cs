using System;
using System.IO;
using System.Net.Mime;
using Mayday.Game.Graphics;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public class SplashScreen : IScreen
    {
        public string Name { get; set; } = "Splash";

        public IScreenManager ScreenManager { get; set; }
        public Color BackgroundColor { get; set; } = Color.White;
        
        public IAnimation ballAnimation;

        private readonly Sprite _logoSprite;
        private readonly Vector2 _logoSpritePos;
        private readonly Texture2D _ballImage;

        private bool _isReady;

        private float _spentTime;
        private float _transValue;
        private const float StayTime = 5f; //seconds

        private bool _shouldRotate;
        private float _angle; // used for rotation
        private float _angleIncreaseExp;
        private float _scale = 1f;

        public SplashScreen()
        {
            _logoSprite = new Sprite(Game1.ContentManager.Load<Texture2D>("Splash/splash"));
            _logoSpritePos = Window.Center; 
            _ballImage = Game1.ContentManager.Load<Texture2D>("Ball");
        }

        public void Awake()
        {
            Game1.InputManager.RegisterInputEvent("interact", OnInteractPressed);
            Game1.InputManager.RegisterInputEvent("secret", OnRotatePressed);
            ballAnimation = new Animation(_ballImage);
            ballAnimation.Initialize(File.ReadAllText("Content/Assets/Ball.json"));
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

        public void Begin()
        {
            _isReady = true;
        }

        public void Update()
        {
            ballAnimation.Update();
            
            if (!_isReady) return;

            if (_shouldRotate)
            {
                _angle += 0.01f * _angleIncreaseExp;
                _angleIncreaseExp += 0.05f;
                _scale -= 0.002f;
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
        }

        public void Draw()
        {
            GraphicsUtils.Draw(_logoSprite, _logoSpritePos, _angle, _scale, Color.White*_transValue);
            ballAnimation.Draw();
        }
        
    }
}