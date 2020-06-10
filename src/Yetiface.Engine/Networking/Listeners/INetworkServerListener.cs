using System;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Yetiface.Engine.Networking.Listeners
{
    public interface INetworkServerListener
    {
        void OnNewConnection(Connection connection, ConnectionInfo info);
        void OnConnectionLeft(Connection connection, ConnectionInfo info);
        void OnMessageReceived(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel);
        void OnConnectionChanged(Connection connection, ConnectionInfo info);
        void AddConsumer(IPacketConsumer packetConsumer);
    }
}