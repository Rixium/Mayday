using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class PlayerPositionPacketConsumer : PacketConsumer<PlayerPositionPacket>
    {
        private readonly Dictionary<ulong, Player> _players;

        public PlayerPositionPacketConsumer(Dictionary<ulong, Player> players)
        {
            _players = players;
        }

        protected override void ConsumePacket(Connection connection, PlayerPositionPacket packet)
        {
            if (!_players.ContainsKey(packet.SteamId)) return;

            var player = _players[packet.SteamId];
            player.X = packet.X;
            player.Y = packet.Y;
        }
    }
}