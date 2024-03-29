﻿using Microsoft.Xna.Framework;

namespace Yetiface.Engine.Utils
{
    public static class Time
    {

        /// <summary>
        /// Get the time passed since last frame.
        /// </summary>
        public static float DeltaTime => (float) GameTime.ElapsedGameTime.TotalSeconds;
        
        /// <summary>
        /// The time scale for delta time.
        /// </summary>

        public static int TimeScale { get; set; } = 1;

        public static GameTime GameTime { get; set; }

        public static void Update(GameTime gameTime)
        {
            GameTime = gameTime;
        }
        
    }
}