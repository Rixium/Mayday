using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player : IPlayer
    {
        public ulong SteamId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public int HeadId { get; set; } = 1;
        public int BodyId { get; set; } = 1;
        public int LegsId { get; set; } = 1;

        public int XDirection { get; set; }

        public int FacingDirection { get; set; } = 1;
        public IGameWorld GameWorld { get; set; }

        public Vector2 Position => new Vector2(X, Y);
        public Vector2 Center => 
            new Vector2(GetBounds().X + GetBounds().Width / 2.0f, GetBounds().Y + GetBounds().Height / 2.0f);

        private readonly IList<IComponent> _components = new List<IComponent>();

        public void Update()
        {
            foreach (var component in _components)
                component.Update();
        }

        public T GetComponent<T>() where T : IComponent => 
            (T) _components.FirstOrDefault(component => component.GetType() == typeof(T));

        public RectangleF GetBounds() =>
            new RectangleF(X + 18 * Game1.GlobalGameScale, Y + 18 * Game1.GlobalGameScale,
                42 * Game1.GlobalGameScale - 17 * Game1.GlobalGameScale - 18 * Game1.GlobalGameScale,
                33 * Game1.GlobalGameScale - 19 * Game1.GlobalGameScale);

        public IComponent AddComponent(IComponent component)
        {
            component.Player = this;
            _components.Add(component);
            
            component.OnAddedToPlayer();
            
            return component;
        }
        
    }
}