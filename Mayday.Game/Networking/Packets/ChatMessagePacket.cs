using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class ChatMessagePacket : INetworkPacket
    {
        
        public ulong SteamId { get; set; }
        public string Message { get; set; }
        
    }
}