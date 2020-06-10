using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yetiface.Engine.Utils
{
    public static class MouseState
    {

        private static Vector2 _mousePosition;
        private static Rectangle _bounds = new Rectangle(0, 0, 1, 1);
        
        /// <summary>
        /// Bounds of the mouse. Use this for collision within the viewport.
        /// </summary>
        public static Rectangle Bounds(Matrix? supplementMatrix = null)
        {

            var matrix1 = Window.InvertViewportMatrix;
            if (supplementMatrix != null)
                matrix1 *= Matrix.Invert(supplementMatrix.Value);
            
            var relativeToViewport = Vector2.Transform(_mousePosition, matrix1);
            _bounds.X = (int) (relativeToViewport.X);
            _bounds.Y = (int) (relativeToViewport.Y);
            
            return _bounds;
        }
        
        /// <summary>
        /// These bounds should be used for anything relative to the window. Probably UI.
        /// </summary>
        public static Rectangle WindowBounds {
            get
            {
                _bounds.X = (int) (_mousePosition.X);
                _bounds.Y = (int) (_mousePosition.Y);
                
                return _bounds;
            }
        }
        
        public static Microsoft.Xna.Framework.Input.MouseState LastState { get; private set; }
        public static Microsoft.Xna.Framework.Input.MouseState CurrentState { get; private set; }
        
        public static void Update()
        {
            CurrentState = Mouse.GetState();
            
            _mousePosition = Mouse.GetState().Position.ToVector2();
        }

        public static void AfterUpdate()
        {
            LastState = CurrentState;
        }

        public static bool Intersects(Rectangle rectangle) => Bounds().Intersects(rectangle);

    }
}