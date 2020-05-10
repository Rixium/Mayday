using Microsoft.Xna.Framework;

namespace Yetiface.Engine.Utils
{
    public static class Time
    {

        /// <summary>
        /// Get the time passed since last frame.
        /// </summary>
        public static float DeltaTime { get; set; }
        
        /// <summary>
        /// The time scale for delta time.
        /// </summary>

        public static int TimeScale { get; set; } = 1;

        public static void Update(GameTime gameTime)
        {
            DeltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds * TimeScale;
        }
        
    }
}