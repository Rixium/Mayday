using Microsoft.Xna.Framework;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.ECS.Components.Renderables
{
    public class SpriteRenderComponent : Component
    {
    
        public Color Color { get; set; }

        protected ISprite Sprite { get; set; }

        public SpriteRenderComponent(ISprite sprite)
        {
            Color = Color.White;
            Sprite = sprite;
        }

        public override void Update()
        {
            
        }

        public override void Draw() => GraphicsUtils.Instance.Draw(Sprite, Entity.Position, Entity.Rotation, Entity.Scale, Color);
        
    }
}