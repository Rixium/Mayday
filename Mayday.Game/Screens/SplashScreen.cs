using System;
using System.Runtime.CompilerServices;
using Mayday.Game.Graphics;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mayday.Game.Screens
{
    public class SplashScreen : IScreen
    {
        public string Name { get; set; } = "Splash";

        public IScreenManager ScreenManager { get; set; }
        public Color BackgroundColor { get; set; } = Color.White;

        private readonly Sprite _sprite;
        private readonly Vector2 _spritePos;

        private float _spentTime;
        private float _transValue;
        private const float StayTime = 5f; //seconds

        public SplashScreen()
        {
            var image = Game1.ContentManager.Load<Texture2D>("Splash/splash");
            _sprite = new Sprite(image);
            _spritePos = Window.Center;
        }
        
        public void Update()
        {
            if (Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                _spentTime = StayTime;
            }
            
            _spentTime += Time.DeltaTime;
            
            if (_spentTime <= StayTime)
            {
                _transValue += 0.01f;
            }
            else
            {
                _transValue -= 0.01f;
            }

            if (_spentTime > StayTime && Math.Abs(_transValue) < 0.001f)
            {
                ScreenManager.ChangeScreen("MenuScreen");
            }

            _transValue = MathHelper.Clamp(_transValue, 0, 1);
        }

        public void Draw()
        {
            GraphicsUtils.Draw(_sprite, _spritePos, Color.White*_transValue);
        }
        
    }
}