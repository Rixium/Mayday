using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Mayday.Game.Networking.Consumers
{
    public class PlayerPositionPacketConsumer : PacketConsumer<PlayerPositionPacket>
    {
        private readonly GameScreen _gameScreen;

        public PlayerPositionPacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }

        protected override void ConsumePacket(Connection connection, PlayerPositionPacket packet)
        {
            if (!_gameScreen.Players.ContainsKey(packet.SteamId)) return;

            var player = _gameScreen.Players[packet.SteamId];
            player.X = packet.X;
            player.Y = packet.Y;
        }
    }
}