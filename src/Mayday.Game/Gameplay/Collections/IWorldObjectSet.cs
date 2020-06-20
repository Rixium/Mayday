using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public interface IWorldObjectSet
    {
        IEntity Add(IEntity worldObject);
        IEntity Get(ulong id);
        IEnumerable<IEntity> GetAll();
    }
}