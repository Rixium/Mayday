using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public interface IPlayerSet
    {
        IEntity Add(IEntity player);
        IEntity Get(ulong id);
        IEnumerable<IEntity> GetAll();
    }
}