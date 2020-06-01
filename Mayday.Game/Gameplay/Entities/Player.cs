using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player : IPlayer
    {
        public ulong SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int HeadId { get; set; } = 1;
        public int BodyId { get; set; } = 1;
        public int LegsId { get; set; } = 1;
        public int ArmsId { get; set; } = 1;

        public int XDirection { get; set; }

        public int FacingDirection { get; set; } = 1;
        public IGameWorld GameWorld { get; set; }

        public Vector2 Position => new Vector2(X, Y);

        private readonly IList<IComponent> _components = new List<IComponent>();

        public void Update()
        {
            foreach (var component in _components)
                component.Update();
        }

        public T GetComponent<T>() where T : IComponent => 
            (T) _components.FirstOrDefault(component => component.GetType() == typeof(T));

        public Rectangle GetBounds() =>
            new Rectangle(X + 18, Y + 18,
                42 - 17 - 18,
                33 - 19);

        public IComponent AddComponent(IComponent component)
        {
            component.Player = this;
            _components.Add(component);
            return component;
        }
        
    }
}