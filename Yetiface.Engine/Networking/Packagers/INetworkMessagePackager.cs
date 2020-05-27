using System;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public interface INetworkMessagePackager
    {
        void AddDefinition(IPacketDefinition packetDefinition);
        byte[] Package<T>(T data) where T : INetworkPacket;
        INetworkPacket Unpack(IntPtr data, int size);
        INetworkPacket Unpack(byte[] data);
    }
}