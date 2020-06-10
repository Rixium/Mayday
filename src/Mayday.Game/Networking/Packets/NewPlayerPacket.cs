using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class NewPlayerPacket : INetworkPacket
    {
        
        public ulong SteamId { get; set; }
        public int HeadId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}