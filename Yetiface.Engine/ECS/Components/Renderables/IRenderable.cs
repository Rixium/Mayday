using Microsoft.Xna.Framework;
using Yetiface.Engine.Graphics;

namespace Yetiface.Engine.ECS.Components.Renderables
{
    public interface IRenderable : IComponent
    {
        /// <summary>
        /// Can override this with some kind of sprite, as long as it's set!
        /// </summary>
        ISprite Sprite { get; set; }
        
        /// <summary>
        /// Whatever color you want the sprite to be tinted.
        /// </summary>
        Color Color { get; set; }
    }
}