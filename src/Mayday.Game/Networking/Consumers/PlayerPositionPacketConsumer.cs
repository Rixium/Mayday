using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class PlayerPositionPacketConsumer : PacketConsumer<PlayerPositionPacket>
    {
        private readonly IEntitySet _entitySet;

        public PlayerPositionPacketConsumer(IEntitySet entitySet)
        {
            _entitySet = entitySet;
        }

        protected override void ConsumePacket(Connection connection, PlayerPositionPacket packet)
        {
            var player = _entitySet.Get(packet.SteamId);

            if (player == null) return;

            player.X = packet.X;
            player.Y = packet.Y;
        }
    }
}