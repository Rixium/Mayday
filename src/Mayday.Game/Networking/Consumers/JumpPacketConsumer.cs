using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class JumpPacketConsumer : PacketConsumer<JumpPacket>
    {
        private readonly IPlayerSet _playerSet;

        public JumpPacketConsumer(IPlayerSet playerSet)
        {
            _playerSet = playerSet;
        }
        
        protected override void ConsumePacket(Connection connection, JumpPacket packet)
        {
            var player = _playerSet.Get(packet.SteamId);

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