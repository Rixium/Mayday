using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class JumpPacket : INetworkPacket
    {
        public ulong SteamId { get; set; }
        public bool IsStopping { get; set; }
        
    }
}