using System.Collections.Generic;
using Yetiface.Engine.ECS.Components.Updateables;
using Yetiface.Engine.Graphics;

namespace Yetiface.Engine.ECS.Components.Renderables
{
    public class Animation : SpriteRenderComponent, IUpdateable
    {
        public IList<ISprite> Sprites { get; }
        public float FrameDuration { get; set; } = 0.150f;
        public int FrameCount => Sprites.Count;

        public Animation(IList<ISprite> sprites) : base(null)
        {
            Sprites = sprites;
        }

        public void Update()
        {
        }
    }
}