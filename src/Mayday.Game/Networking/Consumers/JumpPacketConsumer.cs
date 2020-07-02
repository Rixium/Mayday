using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class JumpPacketConsumer : PacketConsumer<JumpPacket>
    {
        private readonly IEntitySet _entitySet;

        public JumpPacketConsumer(IEntitySet entitySet)
        {
            _entitySet = entitySet;
        }
        
        protected override void ConsumePacket(Connection connection, JumpPacket packet)
        {
            var player = _entitySet.Get(packet.SteamId);

            if (player == null) return;
            
            var jumpComponent = player.GetComponent<JumpComponent>();
            
            if (packet.IsStopping)
            {
                jumpComponent.EndJump();
            }
            else
            {
                jumpComponent.BeginJump();
            }
        }
        
    }
}