using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yetiface.Engine.Utils
{
    public static class MouseState
    {

        private static Vector2 _mousePosition;
        private static float _lastMouseX;
        private static float _lastMouseY;

        private static Rectangle _bounds = new Rectangle(0, 0, 1, 1);
        
        /// <summary>
        /// Bounds of the mouse. Use this for collision within the window.
        /// </summary>
        public static Rectangle Bounds {
            get
            {
                if (!_isDirty) 
                    return _bounds;

                var relativeToViewport = Vector2.Transform(_mousePosition, Window.InvertViewportMatrix);
                _bounds.X = (int) (relativeToViewport.X);
                _bounds.Y = (int) (relativeToViewport.Y);
                
                _isDirty = false;

                return _bounds;
            }
        }

        private static bool _isDirty;
        
        public static void Update()
        {
            _mousePosition = Mouse.GetState().Position.ToVector2();

            if (Math.Abs(_mousePosition.X - _lastMouseX) > 0.01f || Math.Abs(_mousePosition.Y - _lastMouseY) > 0.01f) 
                _isDirty = true;

            _lastMouseX = _mousePosition.X;
            _lastMouseY = _mousePosition.Y;
        }

        public static bool Intersects(Rectangle rectangle) => Bounds.Intersects(rectangle);

    }
}