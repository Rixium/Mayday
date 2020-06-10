using System;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public interface INetworkMessagePackager
    {
        IPacketDefinition AddDefinition<T>() where T : INetworkPacket, new();
        byte[] Package<T>(T data) where T : INetworkPacket;
        INetworkPacket Unpack(IntPtr data, int size);
        INetworkPacket Unpack(byte[] data);
    }
}