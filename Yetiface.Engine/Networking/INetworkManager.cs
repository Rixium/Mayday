using System;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;

namespace Yetiface.Engine.Networking
{
    /// <summary>
    /// Allows for a common abstraction for all types of network management. Be it steam or peer-to-peer by IP.
    /// </summary>
    public interface INetworkManager
    {
        INetworkMessagePackager MessagePackager { get; set; }
        ConnectionManager Client { get; set; }
        SocketManager Server { get; set; }
        SocketManager CreateSession(string port);
        ConnectionManager JoinSession(string ip, string port);

        void Update();

        // TODO Callbacks, not sure if this is ideal.
        void SetServerNetworkListener(INetworkServerListener networkServerListener);
        void SetClientNetworkListener(INetworkClientListener clientNetworkListener);
        void SendMessage(byte[] data);
        void SendMessage(byte[] data, Connection other);
        void RelayMessage(IntPtr data, int size, Connection ignore);
    }
}