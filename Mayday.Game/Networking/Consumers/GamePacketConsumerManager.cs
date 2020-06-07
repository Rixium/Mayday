using System.Collections.Generic;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Screens;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Listeners;

namespace Mayday.Game.Networking.Consumers
{
    public class GamePacketConsumerManager
    {
        
        private readonly IList<IPacketConsumer> _packetConsumers = new List<IPacketConsumer>();

        public GamePacketConsumerManager(GameScreen gameScreen)
        {
            _packetConsumers.Add(new ItemDropPacketConsumer(gameScreen));
            _packetConsumers.Add(new JumpPacketConsumer(gameScreen));
            _packetConsumers.Add(new MovePacketConsumer(gameScreen));
            _packetConsumers.Add(new NewPlayerPacketConsumer(gameScreen));
            _packetConsumers.Add(new PlayerPositionPacketConsumer(gameScreen));
            _packetConsumers.Add(new TileTypePacketConsumer(gameScreen));
            _packetConsumers.Add(new MapRequestPacketConsumer(gameScreen));
        }

        public void InjectInto(MaydayClientNetworkListener gameClientListener = null, INetworkServerListener serverListener = null)
        {
            foreach (var consumer in _packetConsumers)
            {
                serverListener?.AddConsumer(consumer);
                gameClientListener?.AddConsumer(consumer);
            }
        }
        
    }
}