using System;
using System.IO;
using Mayday.Game.Graphics;
using Mayday.Game.UI;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public class SplashScreen : IScreen
    {
        public string Name { get; set; } = "Splash";

        public IScreenManager ScreenManager { get; set; }
        
        public IUserInterface UserInterface { get; set; }
        public Color BackgroundColor { get; set; } = Color.White;
        
        private IAnimation _ballAnimation;

        private Sprite _logoSprite;
        private Vector2 _logoSpritePos;
        private Texture2D _ballImage;

        private bool _isReady;

        private float _spentTime;
        private float _transValue;
        private const float StayTime = 5f; //seconds

        private bool _shouldRotate;
        private float _angle; // used for rotation
        private float _angleIncreaseExp;
        private float _scale = 1f;

        public void Awake()
        {
            _logoSprite = new Sprite(Game1.ContentManager.Load<Texture2D>("Splash/splash"));
            _logoSpritePos = Window.Center; 
            _ballImage = Game1.ContentManager.Load<Texture2D>("Ball");
            
            Game1.InputManager.RegisterInputEvent("interact", OnInteractPressed);
            Game1.InputManager.RegisterInputEvent("secret", OnRotatePressed);
            _ballAnimation = new Animation(_ballImage);
            _ballAnimation.Initialize(File.ReadAllText("Content/Assets/Ball.json"));
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
            _ballAnimation.Update();
            
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
            _ballAnimation.Draw();
        }

        public void Finish()
        {
            // Removing our input events, because if we don't
            // they'll stay in the list on the next screen and will still be called BAD!
            Game1.InputManager.DeRegisterInputEvent("interact", OnInteractPressed);
            Game1.InputManager.DeRegisterInputEvent("secret", OnRotatePressed);
        }
        
    }
}