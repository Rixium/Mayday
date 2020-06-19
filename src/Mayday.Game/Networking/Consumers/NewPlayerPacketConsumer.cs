using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class NewPlayerPacketConsumer : PacketConsumer<NewPlayerPacket>
    {
        private readonly GameScreen _gameScreen;

        public NewPlayerPacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        protected override void ConsumePacket(Connection connection, NewPlayerPacket packet)
        {
            var player = new Entity
            {
                EntityId = packet.SteamId,
                X = packet.X,
                Y = packet.Y
            };

            _gameScreen.AddPlayer(player);
        }
        
    }
}