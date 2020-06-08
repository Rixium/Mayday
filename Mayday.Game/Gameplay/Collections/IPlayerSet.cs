using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public interface IPlayerSet
    {
        Player Add(Player player);
        Player Get(ulong id);
        IEnumerable<Player> GetAll();
    }
}