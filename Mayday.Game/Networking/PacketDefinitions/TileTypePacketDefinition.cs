using System.Text;
using Mayday.Game.Gameplay;
using Mayday.Game.Networking.Packets;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.PacketDefinitions
{
    public class TileTypePacketDefinition : IPacketDefinition
    {
        public int PacketTypeId { get; set; }

        public string Create(object data)
        {
            var packet = (TileTypePacket) data;
            return  $"{packet.TileType}:{packet.X}:{packet.Y}";
        }

        public INetworkPacket Unpack(string data)
        {
            var splitData = data.Split(':');
            return new TileTypePacket()
            {
                TileType = (TileType) int.Parse(splitData[0]),
                X = int.Parse(splitData[1]),
                Y = int.Parse(splitData[2])
            };
        }
        
    }
}