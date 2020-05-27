namespace Yetiface.Engine.Networking.Packets
{
    public interface IPacketDefinition
    {
        int PacketTypeId { get; set; }
        string Create(object data);
        INetworkPacket Unpack(string data);
    }
    
}