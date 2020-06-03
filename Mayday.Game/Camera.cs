using System;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game {

    public class Camera {

        private Matrix _transform;
        private Vector2 _position;

        private int _minX;
        private readonly int _maxX;
        private int _minY;
        private readonly int _maxY;

        private int _zoom = 3;
        private IPlayer _following;

        public Camera() {
            _position = Vector2.Zero;
            _maxX = 2000000;
            _maxY = 1000000;
            
        }

        public void Goto(Vector2 pos)
        {
            ToGo = pos;
        }

        public Vector2 ToGo { get; set; }

        public void Update()
        {
            _minX = (int) (Window.Center.X / _zoom);
            _minY = (int) (Window.Center.Y / _zoom);
            
            if (_following != null)
                Goto(new Vector2(_following.GetBounds().X + _following.GetBounds().Width / 2.0f,
                    _following.GetBounds().Y + _following.GetBounds().Height / 2.0f));
            
            var dir = ToGo - Position;
            dir.Normalize();
            var moveVector = Vector2.Lerp(Position, ToGo,0.05f);
            
            _position.X = moveVector.X;
            _position.Y = moveVector.Y;
            
            _position.X = MathHelper.Clamp(_position.X, _minX, _maxX);
            _position.Y = MathHelper.Clamp(_position.Y, _minY, _maxY);

            if (Math.Abs(_position.X - (int)_position.X + 0.5f) < 0.1f)
                _position.X += 0.1f;
            if (Math.Abs(_position.Y - (int)_position.Y + 0.5f) < 0.1f)
                _position.Y += 0.1f;
        }
        
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Matrix GetMatrix() =>
            Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) *
            Matrix.CreateScale(_zoom, _zoom, 1) *
            Matrix.CreateTranslation(new Vector3(Window.Center.X, Window.Center.Y, 0));

        public void SetEntity(IPlayer myPlayer)
        {
            _following = myPlayer;
        }
    }

}
