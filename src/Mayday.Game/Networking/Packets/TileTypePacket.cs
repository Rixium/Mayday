using Mayday.Game.Gameplay.World;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class TileTypePacket : INetworkPacket
    {
        public int TileType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}