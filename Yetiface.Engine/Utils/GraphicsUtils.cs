using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Graphics;

namespace Yetiface.Engine.Utils
{
    public class GraphicsUtils
    {

        // We wanna be able to access this everywhere.
        // I hate this but there should only ever be one sprite batch so.. yeh.
        public SpriteBatch SpriteBatch;

        /// <summary>
        /// Used with Debug Draw calls so we don't need to continue making rectangles.
        /// </summary>
        private Rectangle _temporaryRectangle = new Rectangle(0, 0, 0, 0);
        
        /// <summary>
        /// Pixel texture is for rendering things like lines.
        /// </summary>
        public Texture2D PixelTexture { get; set; }

        private static GraphicsUtils _graphicsUtils;

        public static GraphicsUtils Instance => _graphicsUtils ?? (_graphicsUtils = new GraphicsUtils());
        public SpriteFont DebugFont { get; set; }

        private GraphicsUtils()
        {
        }
        
        /// <summary>
        /// Wrapping this so we don't have to stare at the ugly ass.
        /// </summary>
        public void Begin()
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

        public void Draw(ISprite sprite, Vector2 position) => Draw(sprite, position, Color.White);

        public void Draw(ISprite sprite, Vector2 position, Color color) => Draw(sprite, position, 0, color);

        public void Draw(ISprite sprite, Vector2 position, float rotation, Color color) =>
            Draw(sprite, position, rotation, 1, color);
        
        public void Draw(ISprite sprite, Vector2 position, float rotation, float scale, Color color)
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
        
        public void End() => SpriteBatch.End();

        public void DrawRectangle(int x, int y, int width, int height, Color color, bool fill = false)
        {
            if (fill)
                DrawFilledRectangle(x, y, width, height, color);
            else 
                DrawHollowRectangle(x, y, width, height, color);
        }

        public void DrawHollowRectangle(int x, int y, int width, int height, Color color)
        {
            _temporaryRectangle.X = x;
            _temporaryRectangle.Y = y;
            _temporaryRectangle.Width = width;
            _temporaryRectangle.Height = 1;
            
            SpriteBatch.Draw(PixelTexture, _temporaryRectangle, color);

            _temporaryRectangle.X = x + width - 1;
            _temporaryRectangle.Y = y;
            _temporaryRectangle.Width = 1;
            _temporaryRectangle.Height = height;
                
            SpriteBatch.Draw(PixelTexture, _temporaryRectangle, color);
            
            _temporaryRectangle.X = x;
            _temporaryRectangle.Y = y + height - 1;
            _temporaryRectangle.Width = width;
            _temporaryRectangle.Height = 1;
            
            SpriteBatch.Draw(PixelTexture, _temporaryRectangle, color);
            
            _temporaryRectangle.X = x;
            _temporaryRectangle.Y = y;
            _temporaryRectangle.Width = 1;
            _temporaryRectangle.Height = height;
            
            SpriteBatch.Draw(PixelTexture, _temporaryRectangle, color);
        }

        public void DrawFilledRectangle(int x, int y, int width, int height, Color color)
        {
            _temporaryRectangle.X = x;
            _temporaryRectangle.Y = y;
            _temporaryRectangle.Width = width;
            _temporaryRectangle.Height = height;
            
            SpriteBatch.Draw(PixelTexture, _temporaryRectangle, color);
        }

        public void Load(ContentManager content)
        {
            PixelTexture = content.Load<Texture2D>("Utils/pixel");
            DebugFont = content.Load<SpriteFont>("Utils/debugFont");
        }
    }
}