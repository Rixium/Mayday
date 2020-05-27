namespace Yetiface.Engine.Networking.Packets
{
    public interface IPacketDefinition
    {
        byte[] Create(object data);

    }
    
}