using Mayday.Game.Gameplay;
using Mayday.Game.Networking.Packets;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.PacketDefinitions
{
    public class TileTypePacketDefinition : PacketDefinition
    {
        
        public override string Pack(INetworkPacket data)
        {
            var tile = (TileTypePacket) data;
            return $"{tile.TileType}:{tile.X}:{tile.Y}";
        }

        public override INetworkPacket Unpack(string data)
        {
            var splitData = data.Split(':');
            return new TileTypePacket
            {
                TileType = (TileType) int.Parse(splitData[0]),
                X = int.Parse(splitData[1]),
                Y = int.Parse(splitData[2])
            };
        }
        
    }
}