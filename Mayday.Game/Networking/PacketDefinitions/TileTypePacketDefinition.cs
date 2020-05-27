using Mayday.Game.Networking.Packets;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.PacketDefinitions
{
    public class TileTypePacketDefinition : IPacketDefinition
    {
        public INetworkPacket Create<T>(T value)
        {
            if (value is TileTypeUpdate pp)
                return new TileTypePacket()
                {
                    TileType = pp.TileType,
                    X = pp.TileX,
                    Y = pp.TileY
                };

            return null;
        }
    }
    
}