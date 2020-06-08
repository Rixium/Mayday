using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Collections
{
    public class PlayerSet : IPlayerSet
    {
        private Dictionary<ulong, Player> Players { get; set; }
            = new Dictionary<ulong, Player>();

        public Player Add(Player player)
        {
            Players.Add(player.SteamId, player);
            return player;
        }

        public Player Get(ulong id) => 
            Players.ContainsKey(id) ? Players[id] : null;

        public IEnumerable<Player> GetAll() => 
            Players.Values;
    }
}