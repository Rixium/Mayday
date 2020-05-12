using Microsoft.Xna.Framework;
using Yetiface.Engine.Graphics;

namespace Yetiface.Engine.ECS.Components.Renderables
{
    public class SpriteRenderComponent : IRenderable
    {
        public IEntity Entity { get; set; }
        
        public Color Color { get; set; }

        public ISprite Sprite { get; set; }

        public SpriteRenderComponent(ISprite sprite)
        {
            Color = Color.White;
            Sprite = sprite;
        }

    }
}