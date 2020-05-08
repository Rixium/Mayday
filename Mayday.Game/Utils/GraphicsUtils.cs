using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Utils
{
    public static class GraphicsUtils
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

        public static void Draw(ISprite sprite, Vector2 position) => Draw(sprite, position, Color.White);

        public static void Draw(ISprite sprite, Vector2 position, Color color)
        {
            SpriteBatch.Draw(sprite.Texture, 
                position,
                sprite.SourceRectangle, 
                color, 
                0, 
                sprite.Origin,
                1, 
                SpriteEffects.None, 
                0);
        }
        
        public static void Draw(ISprite sprite, Vector2 position, float rotation, Color color)
        {
            SpriteBatch.Draw(sprite.Texture, 
                position,
                sprite.SourceRectangle, 
                color,
                rotation, 
                sprite.Origin,
                1, 
                SpriteEffects.None, 
                0);
        }
        
        public static void Draw(ISprite sprite, Vector2 position, float rotation, float scale, Color color)
        {
            SpriteBatch.Draw(sprite.Texture, 
                position,
                sprite.SourceRectangle, 
                color,
                rotation, 
                sprite.Origin,
                scale, 
                SpriteEffects.None, 
                0);
        }
        
        public static void End() => SpriteBatch.End();
        
    }
}