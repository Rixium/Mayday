using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class MovePacketConsumer : PacketConsumer<PlayerMovePacket>
    {
        
        private readonly Dictionary<ulong, Player> _players;

        public MovePacketConsumer(Dictionary<ulong, Player> players)
        {
            _players = players;
        }
        
        protected override void ConsumePacket(Connection connection, PlayerMovePacket packet)
        {
            if (!_players.ContainsKey(packet.SteamId)) 
                return;
            
            var player = _players[packet.SteamId];
            player.XDirection = packet.XDirection;
            player.FacingDirection = packet.XDirection != 0 ? packet.XDirection : player.FacingDirection;
        }
        
    }
}