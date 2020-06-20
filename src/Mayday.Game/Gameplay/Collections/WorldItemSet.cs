using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public class WorldItemSet : IWorldItemSet
    {
        public Action WorldItemSetChanged { get; set; }
        public IList<IEntity> Items { get; set; } = new List<IEntity>();

        public IEntity Add(IEntity item)
        {
            Items.Add(item);
            return item;
        }

        public IEnumerable<IEntity> GetItems() => Items;
        public void Clear() => Items.Clear();

        public void AddRange(List<IEntity> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Set(List<IEntity> newList)
        {
            Items = newList;
            WorldItemSetChanged?.Invoke();
        }
    }
}