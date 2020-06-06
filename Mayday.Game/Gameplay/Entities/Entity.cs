
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{
    public abstract class Entity : IEntity
    {
        public IGameWorld GameWorld { get; set; }
        public int XDirection { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Position => new Vector2(X, Y);
        
        public Vector2 Center => 
            new Vector2(GetBounds().X + GetBounds().Width / 2.0f, GetBounds().Y + GetBounds().Height / 2.0f);

        private readonly IList<IComponent> _components = new List<IComponent>();
        
        public abstract RectangleF GetBounds();
        
        public virtual void Update()
        {
            foreach (var component in _components)
                component.Update();
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            component.Entity = this;
            _components.Add(component);
            
            component.OnAddedToPlayer();
            
            return component;
        }

        public T GetComponent<T>() where T : IComponent =>
            (T) _components.FirstOrDefault(component => component.GetType() == typeof(T));
        
    }
}