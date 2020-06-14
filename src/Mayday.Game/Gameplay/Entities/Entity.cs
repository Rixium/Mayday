
using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;
using IUpdateable = Mayday.Game.Gameplay.Components.IUpdateable;

namespace Mayday.Game.Gameplay.Entities
{
    public abstract class Entity : IEntity
    {
        public ulong EntityId { get; set; }
        public Action<IEntity> Destroy { get; set; }
        public IGameWorld GameWorld { get; set; }
        public int XDirection { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public Vector2 Position => new Vector2(X, Y);
        
        public Vector2 Center => 
            new Vector2(GetBounds().X + GetBounds().Width / 2.0f, GetBounds().Y + GetBounds().Height / 2.0f);

        public int FacingDirection { get; set; }

        protected readonly IList<IComponent> Components = new List<IComponent>();
        protected readonly IList<IUpdateable> UpdateableComponents = new List<IUpdateable>();
        
        public abstract RectangleF GetBounds();
        
        public virtual void Update()
        {
            foreach (var component in UpdateableComponents)
                component.Update();
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            component.Entity = this;
            
            if (typeof(IUpdateable).IsAssignableFrom(typeof(T)))
            {
                UpdateableComponents.Add(component as IUpdateable);
            }
            else
            {
                Components.Add(component);
            }

            component.OnAddedToEntity();
            
            return component;
        }

        public T GetComponent<T>() where T : IComponent
        {
            var c = (T) Components.FirstOrDefault(component => component.GetType() == typeof(T));

            if (c == null)
                c = (T) UpdateableComponents.FirstOrDefault(component => component.GetType() == typeof(T));

            return c;
        }
    }
}