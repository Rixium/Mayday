using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.ECS.Components.Renderables;

namespace Mayday.Game.Graphics
{
    public class SpriteSheet
    {
        
        public Texture2D Texture { get; set; }
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
        
    }
}