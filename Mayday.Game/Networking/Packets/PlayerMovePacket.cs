using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class PlayerMovePacket : INetworkPacket
    {
        
        public ulong SteamId { get; set; }
        public int XDirection { get; set; }
        public int YDirection { get; set; }
        
    }
}