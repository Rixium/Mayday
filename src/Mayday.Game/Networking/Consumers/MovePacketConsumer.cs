using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class MovePacketConsumer : PacketConsumer<PlayerMovePacket>
    {
        private readonly IEntitySet _entitySet;

        public MovePacketConsumer(IEntitySet entitySet)
        {
            _entitySet = entitySet;
        }

        protected override void ConsumePacket(Connection connection, PlayerMovePacket packet)
        {
            var player = _entitySet.Get(packet.SteamId);

            if (player == null) return;

            player.XDirection = packet.XDirection;
            player.FacingDirection = packet.XDirection != 0 ? packet.XDirection : player.FacingDirection;
        }
    }
}