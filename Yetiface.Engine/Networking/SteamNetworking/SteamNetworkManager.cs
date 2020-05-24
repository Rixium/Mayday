using System;
using System.Collections.Generic;
using System.Diagnostics;
using Steamworks;
using Steamworks.Data;

namespace Yetiface.Engine.Networking.SteamNetworking
{
    public class SteamNetworkManager : INetworkManager, ISocketManager
    {
        private readonly uint _appId;
        
        public Dictionary<ulong, string> ConnectedUsers { get; set; } = new Dictionary<ulong, string>();

        public SteamNetworkManager(uint appId)
        {
            _appId = appId;
        }

        public void CreateSession(Action lobbyCreatedCallback)
        {
            SteamNetworkingSockets.CreateNormalSocket(NetAddress.AnyIp(25565), this);
            ConnectedUsers.Add(SteamClient.SteamId, SteamFriends.GetPersona());
            lobbyCreatedCallback?.Invoke();
        }

        public Action OnUserJoined { get; set; }

        public void OnConnecting(Connection connection, ConnectionInfo info)
        {
            connection.Accept();
            Console.WriteLine($"{info.Identity} is connecting");
        }

        public void OnConnected(Connection connection, ConnectionInfo info)
        {
            Console.WriteLine($"{info.Identity} has joined the game");
            
            ConnectedUsers.Add(info.Identity.SteamId.Value, SteamFriends.GetFriendPersona(info.Identity.SteamId.Value));
            OnUserJoined?.Invoke();
        }

        public void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            Console.WriteLine($"{info.Identity} is out of here");
        }

        public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
            long recvTime,
            int channel)
        {
            Console.WriteLine($"We got a message from {identity}!");

            // Send it right back
            connection.SendMessage(data, size);
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