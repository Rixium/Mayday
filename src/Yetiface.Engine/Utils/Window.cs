using Microsoft.Xna.Framework;

namespace Yetiface.Engine.Utils
{
    public static class Window
    {

        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static int ViewportWidth { get; set; } = 1280;
        public static int ViewportHeight { get; set; } = 720;
        public static int WindowWidth { get; set; } = 1280;
        public static int WindowHeight { get; set; } = 720;

        /// <summary>
        /// Returns the center of the viewport.
        /// </summary>
        public static Vector2 Center => new Vector2(WindowWidth / 2.0f, WindowHeight / 2.0f);

        public static Vector2 BottomRight => new Vector2(ViewportWidth, ViewportHeight);

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
        public static void ResizeWindow(int width, int height)
        {
            var windowClientBounds = new Rectangle(0, 0, width, height);
            UpdateWindowSize(windowClientBounds);
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
        public static GraphicsDeviceManager CreateGraphicsDevice(YetiGame game)
            => new GraphicsDeviceManager(game)
            {
                PreferredBackBufferWidth = WindowWidth,
                PreferredBackBufferHeight = WindowHeight,
            };
        
    }
}