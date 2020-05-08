using Microsoft.Xna.Framework;

namespace Mayday.Game.Utils
{
    public static class Window
    {

        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static int ViewportWidth { get; set; } = 1280;
        public static int ViewportHeight { get; set; } = 720;
        public static int WindowWidth { get; set; } = 1280;
        public static int WindowHeight { get; set; } = 720;
        
        private static Matrix _viewportMatrix = Matrix.CreateScale(1, 1, 1.0f);

        /// <summary>
        /// The viewport relative to the window width.
        /// Important for resizing the window --> We want everything to scale correctly.
        /// </summary>
        public static Matrix ViewportMatrix
        {
            get
            {
                _viewportMatrix.M11 = (float) WindowWidth / ViewportWidth;
                _viewportMatrix.M22 = (float) WindowHeight / ViewportHeight;
                return _viewportMatrix;
            }
        }

        /// <summary>
        /// The inverse of the above, so we can remove any translations for stuff like input.
        /// </summary>
        public static Matrix InvertViewportMatrix => Matrix.Invert(ViewportMatrix);

        /// <summary>
        /// Returns the center of the viewport.
        /// </summary>
        public static Vector2 Center => new Vector2(ViewportWidth / 2.0f, ViewportHeight / 2.0f);

        /// <summary>
        /// Sets all the properties required and makes sure that the window maintains aspect.
        /// </summary>
        /// <param name="windowClientBounds"></param>
        public static void UpdateWindowSize(Rectangle windowClientBounds)
        {
            var newWidth = windowClientBounds.Width;
            var newHeight = windowClientBounds.Height;

            var oldHeight = WindowHeight;
            var oldWidth = WindowWidth;
                
            if (newWidth != oldWidth)
            {
                WindowHeight = (int) (newWidth / (16.0f / 9));
                WindowWidth = newWidth;
            }
                
            if (newHeight != oldHeight)
            {
                WindowWidth = (int) (newHeight * (16.0f / 9));
                WindowHeight = newHeight;
            }
        }

        /// <summary>
        /// Pushes the window size change to the game.
        /// </summary>
        /// <param name="window">The window that has been resized.</param>
        public static void WindowResized(GameWindow window)
        {
            UpdateWindowSize(window.ClientBounds);
            ApplyWindowSize();
        }

        /// <summary>
        /// Applies the current window size settings.
        /// </summary>
        private static void ApplyWindowSize()
        {
            GraphicsDeviceManager.PreferredBackBufferHeight = WindowHeight;
            GraphicsDeviceManager.PreferredBackBufferWidth = WindowWidth;
            GraphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Create graphics device manager with the height and width as set in this class.
        /// </summary>
        /// <param name="game">The game for the graphics device manager.</param>
        /// <returns>A set up graphics device manager.</returns>
        public static GraphicsDeviceManager CreateGraphicsDevice(Game1 game)
            => new GraphicsDeviceManager(game)
            {
                PreferredBackBufferWidth = WindowWidth,
                PreferredBackBufferHeight = WindowHeight
            };
        
    }
}