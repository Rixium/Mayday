using System.Threading.Tasks;
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
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        private readonly IGameWorld _gameWorld;

        public MapRequestPacketConsumer(INetworkManager networkManager,
            INetworkMessagePackager messagePackager, IGameWorld gameWorld)
        {
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
                    var tileToSend = _gameWorld.Tiles[i, j];
                    var tileTypePacket = new TileTypePacket()
                    {
                        X = tileToSend.X,
                        Y = tileToSend.Y,
                        TileType = tileToSend.TileType
                    };
                    var packet = _messagePackager.Package(tileTypePacket);
                    _networkManager.SendMessage(packet, connection);
                }

                await Task.Delay(1);
            }
        }
        
    }
}