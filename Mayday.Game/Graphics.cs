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
            SpriteBatch.Begin(SpriteSortMode.Deferred, 
                null, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullClockwise, 
                null, 
                Utils.Window.ViewportMatrix);
        }

        public static void End() => SpriteBatch.End();
        
    }
}