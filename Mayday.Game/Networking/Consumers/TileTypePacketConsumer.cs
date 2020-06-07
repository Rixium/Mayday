using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class TileTypePacketConsumer : PacketConsumer<TileTypePacket>
    {
        private readonly IGameWorld _gameWorld;

        public TileTypePacketConsumer(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        protected override void ConsumePacket(Connection connection, TileTypePacket packet)
        {
            _gameWorld.Tiles[packet.X, packet.Y].TileType = packet.TileType;
        }
    }
}