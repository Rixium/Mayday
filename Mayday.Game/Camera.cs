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

        public Camera() {
            _position = Vector2.Zero;
            _maxX = 2000000;
            _maxY = 1000000;
        }

        public void Move(Vector2 amount)
        {
            _position += amount * 4;
            _position.X = MathHelper.Clamp(_position.X, _minX, _maxX);
            _position.Y = MathHelper.Clamp(_position.Y, _minY, _maxY);
        }

        public void Goto(Vector2 pos) {
            _minX = Window.WindowWidth / 2 / 2;
            _minY = Window.WindowHeight / 2 / 2;

            ToGo = pos;
        }

        public Vector2 ToGo { get; set; }

        public void Update()
        {
            var dir = ToGo - Position;
            dir.Normalize();

            _position = Vector2.Lerp(Position, ToGo,0.1f);
            
            _position.X = MathHelper.Clamp(_position.X, _minX, _maxX);
            _position.Y = MathHelper.Clamp(_position.Y, _minY, _maxY);
        }
        
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Matrix GetMatrix() {
            _transform =
                Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) *
                Matrix.CreateScale(2, 2, 1) *
                Matrix.CreateTranslation(new Vector3(Window.ViewportWidth / 2.0f, Window.ViewportHeight / 2.0f, 0));

            return _transform;
        }

    }

}
