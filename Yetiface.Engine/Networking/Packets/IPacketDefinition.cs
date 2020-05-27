using System;

namespace Yetiface.Engine.Networking.Packets
{
    public interface IPacketDefinition
    {
        int PacketTypeId { get; set; }
        Type PacketType { get; set; }
        string Pack(INetworkPacket data);
        INetworkPacket Unpack(string data);
    }
    
}