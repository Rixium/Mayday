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
        
        public void CreateSession(Action lobbyCreatedCallback)
        {
            var myServer = new MyServer();
            myServer.OnNewConnection += OnUserJoined;
            SteamNetworkingSockets.CreateNormalSocket(NetAddress.AnyIp(25565), myServer);
            lobbyCreatedCallback?.Invoke();
        }

        public Action OnUserJoined { get; set; }
    }

    public class MyServer : ISocketManager
    {
        public void OnConnecting(Connection connection, ConnectionInfo data)
        {
            connection.Accept();
            Debug.WriteLine($"{data.Identity} is connecting");
        }

        public void OnConnected(Connection connection, ConnectionInfo data)
        {
            Debug.WriteLine($"{data.Identity} has joined the game");
            OnNewConnection?.Invoke();
        }

        public void OnDisconnected(Connection connection, ConnectionInfo data)
        {
            Debug.WriteLine($"{data.Identity} is out of here");
        }

        public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size,
            long messageNum, long recvTime, int channel)
        {
            Debug.WriteLine($"We got a message from {identity}!");

            // Send it right back
            connection.SendMessage(data, size, SendType.Reliable);
        }

        public Action OnNewConnection { get; set; }
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