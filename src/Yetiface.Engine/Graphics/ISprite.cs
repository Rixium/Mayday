using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yetiface.Engine.Graphics
{
    
    public interface ISprite
    {
        
        Texture2D Texture { get; set; }
        Rectangle? SourceRectangle { get; set; }
        Vector2 Origin { get; set; }
        
    }
    
}