using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class TileTypePacket : INetworkPacket
    {
        public string TileType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}