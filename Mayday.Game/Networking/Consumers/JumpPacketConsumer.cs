using System.Collections.Generic;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class JumpPacketConsumer : PacketConsumer<JumpPacket>
    {
        private readonly GameScreen _gameScreen;
        private readonly Dictionary<ulong, Player> _players;

        public JumpPacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        protected override void ConsumePacket(Connection connection, JumpPacket packet)
        {
            var player = _gameScreen.Players[packet.SteamId];
            
            var jumpComponent = player.GetComponent<JumpComponent>();
            
            if (packet.IsStopping)
            {
                jumpComponent.EndJump();
            }
            else
            {
                jumpComponent.Jump();
            }
        }
        
    }
}