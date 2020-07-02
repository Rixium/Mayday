using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public interface IEntitySet
    {
        IEntity Add(IEntity entity);
        IEntity Get(ulong id);
        IEnumerable<IEntity> GetAll();
    }
}