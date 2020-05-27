namespace Yetiface.Engine.Networking.Packets
{
    public interface IPacketDefinition
    {
        INetworkPacket Create<T>(T value);
    }
    
}