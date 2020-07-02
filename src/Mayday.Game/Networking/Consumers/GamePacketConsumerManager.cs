using System.Collections.Generic;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Screens;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.Consumers
{
    public class GamePacketConsumerManager
    {
        
        private readonly IList<IPacketConsumer> _packetConsumers = new List<IPacketConsumer>();

        public GamePacketConsumerManager(
            IEntitySet entitySet,
            GameScreen gameScreen,
            IGameWorld gameWorld,
            INetworkMessagePackager messagePackager,
            INetworkManager networkManager)
        {
            _packetConsumers.Add(new ItemDropPacketConsumer(gameWorld));
            _packetConsumers.Add(new JumpPacketConsumer(entitySet));
            _packetConsumers.Add(new MovePacketConsumer(entitySet));
            _packetConsumers.Add(new NewPlayerPacketConsumer(gameScreen));
            _packetConsumers.Add(new PlayerPositionPacketConsumer(entitySet));
            _packetConsumers.Add(new TileTypePacketConsumer(gameWorld));
            _packetConsumers.Add(new MapRequestPacketConsumer(entitySet, networkManager, messagePackager, gameWorld));
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