
using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.World.Areas;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;
using IUpdateable = Mayday.Game.Gameplay.Components.IUpdateable;

namespace Mayday.Game.Gameplay.Entities
{
    public class Entity : IEntity
    {
        public ulong EntityId { get; set; }
        public Action<IEntity> Destroy { get; set; }
        public IGameWorld GameWorld { get; set; }
        public IGameArea GameArea { get; set; }
        public int XDirection { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public Vector2 Position => new Vector2(X, Y);
        
        public Vector2 Center => 
            new Vector2(GetCurrentBounds().X + GetCurrentBounds().Width / 2.0f,
                GetCurrentBounds().Y + GetCurrentBounds().Height / 2.0f);

        public int FacingDirection { get; set; }

        protected IList<IComponent> Components;
        protected IList<IUpdateable> UpdateableComponents;

        public RectangleF Bounds { get; set; }

        public virtual RectangleF GetCurrentBounds() =>
            new RectangleF(Bounds.X + X, Bounds.Y + Y, Bounds.Width, Bounds.Height);

        public Entity(ulong entityId) => EntityId = entityId;

        public virtual void Update()
        {
            if (UpdateableComponents == null) return;

            foreach (var component in UpdateableComponents)
                component.Update();
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            Components ??= new List<IComponent>();

            component.Entity = this;
            
            if (typeof(IUpdateable).IsAssignableFrom(typeof(T)))
            {
                UpdateableComponents ??= new List<IUpdateable>();
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
            var c = (T) Components?.FirstOrDefault(component => component.GetType() == typeof(T));

            if (c == null)
                c = (T) UpdateableComponents?.FirstOrDefault(component => component.GetType() == typeof(T));

            return c;
        }

        protected void CleanUpComponents()
        {
            Components?.Clear();
            UpdateableComponents?.Clear();
        }
        
    }
}