﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Mayday.Game.Graphics;
using Mayday.Game.Screens.Transitions;
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
            var image = Game1.ContentManager.Load<Texture2D>("Splash/splash");
            _sprite = new Sprite(image);
            _spritePos = Window.Center;
        }

        public void Awake()
        {
            
        }

        public void Begin()
        {
            _isReady = true;
        }

        public void Update()
        {
            if (!_isReady) return;
            
            if (Keyboard.GetState().GetPressedKeys().Length > 0
                && !Keyboard.GetState().GetPressedKeys().Contains(Keys.P)) 
            {
                _spentTime = StayTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                _shouldRotate = true;
            }

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
            GraphicsUtils.Draw(_sprite, _spritePos, _angle, _scale, Color.White*_transValue);
        }
        
    }
}