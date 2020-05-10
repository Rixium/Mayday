using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yetiface.Engine.Graphics
{
    public class Sprite : ISprite
    {
        
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Vector2 Origin { get; set; }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        }
        
        public Sprite(Texture2D texture, Rectangle? sourceRectangle, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
            SourceRectangle = sourceRectangle;
        }

        public Sprite(Texture2D texture, Rectangle sourceRectangle) : this(texture, sourceRectangle, new Vector2(sourceRectangle.Width / 2.0f, sourceRectangle.Height / 2.0f))
        {
            
        }
    }
}