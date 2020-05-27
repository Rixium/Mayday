using System;

namespace Yetiface.Engine.Networking.Packets
{
    public abstract class PacketDefinition : IPacketDefinition
    {
        private static int _packetTypeIdCounter;
        public int PacketTypeId { get; set; } = _packetTypeIdCounter++;
        public Type PacketType { get; set; }

        public abstract string Pack(INetworkPacket data);

        public abstract INetworkPacket Unpack(string data);
    }
}