using System.Text;
using Mayday.Game.Networking.Packets;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.PacketDefinitions
{
    public class TileTypePacketDefinition : IPacketDefinition
    {
        public byte[] Create(object data)
        {
            var packet = (TileTypePacket) data;
            var dString = $"{packet.TileType}:{packet.X}:{packet.Y}";
            return Encoding.UTF8.GetBytes(dString);
        }
        
    }
}