using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.ECS.Components;
using Yetiface.Engine.Screens;

namespace Yetiface.Engine.ECS
{
    public class Entity : IEntity
    {
        public Vector2 Position { get; set; }

        // 1 By default, or any new entities will have a zero scale.
        public float Scale { get; set; } = 1.0f;
        public float Rotation { get; set; }
        public IList<IComponent> Components { get; set; }
        public IScreen Screen { get; set; }

        public Entity(IScreen screen) : this(screen, Vector2.Zero)
        {
            // Ignore this, it calls the constructor below.
        }
        
        public Entity(IScreen screen, Vector2 position)
        {
            Screen = screen;
            Position = position;
        }
        
        public void AddComponent(IComponent component)
        {
            if (Components == null)
                Components = new List<IComponent>();

            component.Entity = this;
            
            Screen.AddComponentToSystems(component);
        }
        
    }
}