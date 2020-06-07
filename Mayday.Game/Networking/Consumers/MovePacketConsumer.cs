using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class MovePacketConsumer : PacketConsumer<PlayerMovePacket>
    {
        private readonly GameScreen _gameScreen;

        public MovePacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        protected override void ConsumePacket(Connection connection, PlayerMovePacket packet)
        {
            if (!_gameScreen.Players.ContainsKey(packet.SteamId)) 
                return;
            
            var player = _gameScreen.Players[packet.SteamId];
            player.XDirection = packet.XDirection;
            player.FacingDirection = packet.XDirection != 0 ? packet.XDirection : player.FacingDirection;
        }
        
    }
}