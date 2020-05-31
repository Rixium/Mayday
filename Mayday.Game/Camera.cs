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

        public Camera() {
            _position = Vector2.Zero;
            _maxX = 2000000;
            _maxY = 1000000;
        }

        public void Goto(Vector2 pos)
        {
            _minX = (int) (Window.Center.X / _zoom);
            _minY = (int) (Window.Center.Y / _zoom);

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
                Matrix.CreateScale(_zoom, _zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Window.Center.X, Window.Center.Y, 0));

            return _transform;
        }

    }

}
