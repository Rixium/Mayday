using System.Threading.Tasks;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.Consumers
{
    public class MapRequestPacketConsumer : PacketConsumer<MapRequestPacket>
    {
        private readonly IEntitySet _entitySet;
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        private readonly IGameWorld _gameWorld;

        public MapRequestPacketConsumer(
            IEntitySet entitySet,
            INetworkManager networkManager,
            INetworkMessagePackager messagePackager,
            IGameWorld gameWorld)
        {
            _entitySet = entitySet;
            _networkManager = networkManager;
            _messagePackager = messagePackager;
            _gameWorld = gameWorld;
        }

        protected override void ConsumePacket(Connection connection, MapRequestPacket packet)
        {
            if (connection == default(Connection)) return;
            
            SendMap(connection);
        }

        private async void SendMap(Connection connection)
        {
            for (var i = 0; i < _gameWorld.Width; i++)
            {
                for (var j = 0; j < _gameWorld.Height; j++)
                {
                    // TODO NETWORK GAME AREAS
                    var tileToSend = _gameWorld.GameAreas[0].Tiles[i, j];
                    var tileTypePacket = new TileTypePacket()
                    {
                        X = tileToSend.TileX,
                        Y = tileToSend.TileY,
                        TileType = tileToSend.TileType
                    };
                    var packet = _messagePackager.Package(tileTypePacket);
                    _networkManager.SendMessage(packet, connection);
                }

                await Task.Delay(1);
            }

            foreach (var player in _entitySet.GetAll())
            {
                var playerPacket = new NewPlayerPacket()
                {
                    SteamId = player.EntityId,
                    X = (int) player.X,
                    Y = (int) player.Y,
                    HeadId = 1
                };

                var package = _messagePackager.Package(playerPacket);
                
                _networkManager.SendMessage(package, connection);
                
                await Task.Delay(1);
            }

            var mapSendCompletePacket = new MapSendCompletePacket();
            var mapSendCompletePackage = _messagePackager.Package(mapSendCompletePacket);
            _networkManager.SendMessage(mapSendCompletePackage, connection);
        }
        
    }
}