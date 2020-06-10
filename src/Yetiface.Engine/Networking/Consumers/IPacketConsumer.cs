using System;
using Steamworks.Data;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Consumers
{
    public interface IPacketConsumer
    {
        void Consume(Connection connection, INetworkPacket packet);
        void Consume(INetworkPacket packet);
        Type PacketType { get; set; }
    }
}