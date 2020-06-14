﻿using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game
{
    public class Camera
    {
        private Vector2 _position;

        private int _minX;
        private readonly int _maxX;
        private int _minY;
        private readonly int _maxY;

        private int _zoom = 1;
        private IEntity _following;

        public Camera()
        {
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

            var (x, y) = Vector2.Lerp(Position, ToGo, 0.03f);

            _position.X = x;
            _position.Y = y;

            _position.X = MathHelper.Clamp(_position.X, _minX, _maxX);
            _position.Y = MathHelper.Clamp(_position.Y, _minY, _maxY);
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Matrix GetMatrix() =>
            Matrix.CreateTranslation(new Vector3(-(int) _position.X, -(int) _position.Y, 0)) *
            Matrix.CreateScale(_zoom, _zoom, 1) *
            Matrix.CreateTranslation(new Vector3(Window.Center.X, Window.Center.Y, 0));

        public void SetEntity(IEntity entity)
        {
            _following = entity;
        }

        public bool Intersects(RectangleF bounds)
        {
            var rect = new RectangleF(_position.X - Window.Center.X, _position.Y - Window.Center.Y, Window.ViewportWidth, Window.ViewportHeight);
            return rect.Intersects(bounds);
        }
    }
}