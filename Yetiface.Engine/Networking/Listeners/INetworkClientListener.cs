using System;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;

namespace Yetiface.Engine.Networking.Listeners
{
    public interface INetworkClientListener
    {

        void OnDisconnectedFromServer(ConnectionInfo info);
        void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel);
        void OnConnectedToServer(ConnectionInfo info);
        void AddConsumer(IPacketConsumer packetConsumer);

    }
}