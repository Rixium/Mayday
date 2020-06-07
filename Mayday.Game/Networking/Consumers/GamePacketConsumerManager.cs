using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Screens;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Listeners;

namespace Mayday.Game.Networking.Consumers
{
    public class GamePacketConsumerManager
    {
        
        private readonly IList<IPacketConsumer> _packetConsumers = new List<IPacketConsumer>();

        public GamePacketConsumerManager(GameScreen gameScreen,
            Dictionary<ulong, Player> players, IGameWorld gameWorld)
        {
            _packetConsumers.Add(new ItemDropPacketConsumer(gameScreen));
            _packetConsumers.Add(new JumpPacketConsumer(players));
            _packetConsumers.Add(new MovePacketConsumer(players));
            _packetConsumers.Add(new NewPlayerPacketConsumer(gameScreen));
            _packetConsumers.Add(new PlayerPositionPacketConsumer(players));
            _packetConsumers.Add(new TileTypePacketConsumer(gameWorld));
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