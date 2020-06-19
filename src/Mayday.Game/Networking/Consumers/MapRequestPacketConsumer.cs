using System.Collections.Generic;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.Consumers
{
    public class MapRequestPacketConsumer : PacketConsumer<MapRequestPacket>
    {
        private readonly GameScreen _gameScreen;
        private INetworkManager NetworkManager => _gameScreen.NetworkManager;
        private INetworkMessagePackager MessagePackager => NetworkManager.MessagePackager;
        private IGameWorld GameWorld => _gameScreen.GameWorld;

        public MapRequestPacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }

        protected override void ConsumePacket(Connection connection, MapRequestPacket packet)
        {
            if (connection == default(Connection)) return;
            
            SendMap(connection);
        }

        private async void SendMap(Connection connection)
        {
            for (var i = 0; i < GameWorld.Width; i++)
            {
                for (var j = 0; j < GameWorld.Height; j++)
                {
                    var tileToSend = GameWorld.Tiles[i, j];
                    var tileTypePacket = new TileTypePacket()
                    {
                        X = tileToSend.TileX,
                        Y = tileToSend.TileY,
                        TileType = (int) tileToSend.TileType
                    };
                    var packet = MessagePackager.Package(tileTypePacket);
                    NetworkManager.SendMessage(packet, connection);
                }

                await Task.Delay(1);
            }

            foreach (var player in _gameScreen.Players.GetAll())
            {
                var playerPacket = new NewPlayerPacket()
                {
                    SteamId = player.EntityId,
                    X = (int) player.X,
                    Y = (int) player.Y,
                    HeadId = 1
                };

                var package = MessagePackager.Package(playerPacket);
                
                NetworkManager.SendMessage(package, connection);
                
                await Task.Delay(1);
            }

            var mapSendCompletePacket = new MapSendCompletePacket();
            var mapSendCompletePackage = MessagePackager.Package(mapSendCompletePacket);
            NetworkManager.SendMessage(mapSendCompletePackage, connection);
        }
        
    }
}