using System.Collections.Generic;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class JumpPacketConsumer : PacketConsumer<JumpPacket>
    {
        private readonly Dictionary<ulong, Player> _players;

        public JumpPacketConsumer(Dictionary<ulong, Player> players)
        {
            _players = players;
        }
        
        protected override void ConsumePacket(Connection connection, JumpPacket packet)
        {
            var player = _players[packet.SteamId];
            var jumpComponent = player.GetComponent<JumpComponent>();
            jumpComponent.Jump();
        }
        
    }
}