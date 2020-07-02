using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public class EntitySet : IEntitySet
    {
        private Dictionary<ulong, IEntity> Entities { get; set; }
            = new Dictionary<ulong, IEntity>();

        public IEntity Add(IEntity entity)
        {
            Entities.Add(entity.EntityId, entity);
            return entity;
        }

        public IEntity Get(ulong id) =>
            Entities.ContainsKey(id) ? Entities[id] : null;

        public IEnumerable<IEntity> GetAll() =>
            Entities.Values;
    }
}