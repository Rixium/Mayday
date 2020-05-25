using Steamworks;
using Yetiface.Engine.Networking.SteamNetworking;

namespace Yetiface.Engine.Networking
{
    /// <summary>
    /// Allows for a common abstraction for all types of network management. Be it steam or peer-to-peer by IP.
    /// </summary>
    public interface INetworkManager
    {
        ConnectionManager Client { get; set; }
        SocketManager Server { get; set; }
        SocketManager CreateSession();
        ConnectionManager JoinSession(string ip);

        void Update();

        // TODO Callbacks, not sure if this is ideal.
        void SetServerNetworkListener(INetworkServerListener networkServerListener);
        void SetClientNetworkListener(INetworkClientListener clientNetworkListener);
        void SendMessage(MessageType messageType, string value = "");
    }
}