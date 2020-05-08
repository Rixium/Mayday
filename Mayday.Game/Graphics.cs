using Mayday.Game.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game
{
    public class Graphics
    {

        // We wanna be able to access this everywhere.
        // I hate this but there should only ever be one sprite batch so.. yeh.
        public static SpriteBatch SpriteBatch;

        /// <summary>
        /// Wrapping this so we don't have to stare at the ugly ass.
        /// </summary>
        public static void Begin()
        {
            SpriteBatch.Begin(
                SpriteSortMode.Deferred, // Only render images when end has been called
                null,  // No blending
                SamplerState.PointClamp, // Point clamp, so we get sexy pixel perfect resizing
                null, // We don't care about this. Tbh, I don't even understand it.
                null, // I don't even know what this it.
                null, // We can choose to flip textures as an example, but we dont, so null it.
                Window.ViewportMatrix); // Window viewport, for nice resizing.
        }

        public static void End() => SpriteBatch.End();
        
    }
}