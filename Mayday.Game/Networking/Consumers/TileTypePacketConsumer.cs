using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class TileTypePacketConsumer : PacketConsumer<TileTypePacket>
    {
        private readonly GameScreen _gameScreen;

        public TileTypePacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }

        protected override void ConsumePacket(Connection connection, TileTypePacket packet) => 
            _gameScreen.GameWorld.Tiles[packet.X, packet.Y].TileType = packet.TileType;
        
    }
}