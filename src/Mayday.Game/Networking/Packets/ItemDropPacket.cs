using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class ItemDropPacket : INetworkPacket
    {
        public string ItemId { get; set; }
        public float Y { get; set; }
        public float X { get; set; }
    }
}