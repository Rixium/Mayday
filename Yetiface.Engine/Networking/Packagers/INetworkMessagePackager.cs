using System;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public interface INetworkMessagePackager
    {
        void AddDefinition(Type packetType, IPacketDefinition packetDefinition);
        byte[] Package<T>(T data) where T : INetworkPacket;

    }
}