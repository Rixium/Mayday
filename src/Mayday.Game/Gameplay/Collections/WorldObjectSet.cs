using System.Collections;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public class WorldObjectSet : IWorldObjectSet
    {
        private Dictionary<ulong, IEntity> WorldObjects { get; set; }
            = new Dictionary<ulong, IEntity>();

        public IEntity Add(IEntity worldObject)
        {
            WorldObjects.Add(worldObject.EntityId, worldObject);
            return worldObject;
        }

        public IEntity Get(ulong id) =>
            WorldObjects.ContainsKey(id) ? WorldObjects[id] : null;

        public IEnumerable<IEntity> GetAll() =>
            WorldObjects.Values;

        public IEnumerator<IEntity> GetEnumerator() => GetAll().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}