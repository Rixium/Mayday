using System;
using System.Diagnostics;
using Steamworks;
using Steamworks.Data;

namespace Yetiface.Engine.Networking.SteamNetworking
{
    public class SteamNetworkManager : INetworkManager
    {
        private readonly uint _appId;

        public SteamNetworkManager(uint appId)
        {
            _appId = appId;
        }
        
        public void CreateSession(Action<ILobbyInformation> lobbyCreatedCallback) => 
            SteamNetworkingSockets.CreateNormalSocket<MyServer>(NetAddress.AnyIp(25565));
    }

    public class MyServer : SocketManager
    {
        public override void OnConnecting(Connection connection, ConnectionInfo data)
        {
            connection.Accept();
            Debug.WriteLine($"{data.Identity} is connecting");
        }

        public override void OnConnected(Connection connection, ConnectionInfo data)
        {
            Debug.WriteLine($"{data.Identity} has joined the game");
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo data)
        {
            Debug.WriteLine($"{data.Identity} is out of here");
        }

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size,
            long messageNum, long recvTime, int channel)
        {
            Debug.WriteLine($"We got a message from {identity}!");

            // Send it right back
            connection.SendMessage(data, size, SendType.Reliable);
        }

    }
    
    public class TestConnectionInterface : ConnectionManager
    {
        public override void OnConnectionChanged(ConnectionInfo data)
        {
            Debug.WriteLine($"[Connection][{Connection}] [{data.State}]");

            base.OnConnectionChanged(data);
        }

        public override void OnConnecting(ConnectionInfo data)
        {
            Debug.WriteLine($" - OnConnecting");
            base.OnConnecting(data);
        }

        /// <summary>
        /// Client is connected. They move from connecting to Connections
        /// </summary>
        public override void OnConnected(ConnectionInfo data)
        {
            Debug.WriteLine($" - OnConnected");
            base.OnConnected(data);
        }

        /// <summary>
        /// The connection has been closed remotely or disconnected locally. Check data.State for details.
        /// </summary>
        public override void OnDisconnected(ConnectionInfo data)
        {
            Debug.WriteLine($" - OnDisconnected");
            base.OnDisconnected(data);
        }

    }
}