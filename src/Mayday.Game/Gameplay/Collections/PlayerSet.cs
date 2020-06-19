using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public class PlayerSet : IPlayerSet
    {
        private Dictionary<ulong, IEntity> Players { get; set; }
            = new Dictionary<ulong, IEntity>();

        public IEntity Add(IEntity player)
        {
            Players.Add(player.EntityId, player);
            return player;
        }

        public IEntity Get(ulong id) =>
            Players.ContainsKey(id) ? Players[id] : null;

        public IEnumerable<IEntity> GetAll() =>
            Players.Values;
    }
}