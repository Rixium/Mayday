using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.Collections
{
    public interface IWorldItemSet
    {
        
        IList<IEntity> Items { get; set; }
        IEntity Add(IEntity item);
        IEnumerable<IEntity> GetItems();
        void Clear();
        void AddRange(List<IEntity> items);
        void Set(List<IEntity> newList);
    }
}