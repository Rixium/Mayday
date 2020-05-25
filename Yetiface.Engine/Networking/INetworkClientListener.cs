using System;
using Steamworks.Data;

namespace Yetiface.Engine.Networking
{
    public interface INetworkClientListener
    {

        void OnDisconnectedFromServer(ConnectionInfo info);
        void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel);
        void OnConnectedToServer(ConnectionInfo info);
    }
}