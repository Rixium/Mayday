using Mayday.Game.Gameplay.Components;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class JumpPacketConsumer : PacketConsumer<JumpPacket>
    {
        private readonly GameScreen _gameScreen;

        public JumpPacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        protected override void ConsumePacket(Connection connection, JumpPacket packet)
        {
            var player = _gameScreen.Players.Get(packet.SteamId);

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