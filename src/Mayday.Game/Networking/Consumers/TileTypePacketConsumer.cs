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

        // TODO NETWORKED GAME AREAS
        protected override void ConsumePacket(Connection connection, TileTypePacket packet) => 
            _gameScreen.GameWorld.GameAreas[0].Tiles[packet.X, packet.Y].TileType = packet.TileType;
        
    }
}