using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
using Steamworks.Data;

namespace Yetiface.Engine.Networking.SteamNetworking
{
    public class SteamNetworkManager : INetworkManager
    {
        private readonly uint _appId;
        private MyServer _myServer;

        private List<Connection> Connections { get; set; } = new List<Connection>();

        public SteamNetworkManager(uint appId)
        {
            _appId = appId;
        }

        public Dictionary<ulong, string> ConnectedUsers => _myServer.ConnectedUsers;

        public void CreateSession(Action<MyServer> lobbyCreatedCallback)
        {
            _myServer = SteamNetworkingSockets.CreateNormalSocket<MyServer>(NetAddress.AnyIp(25565));

            _myServer.OnUserJoined += OnUserJoined;
            _myServer.Manager = this;
            
            ConnectedUsers.Add(SteamClient.SteamId, SteamFriends.GetPersona());
            lobbyCreatedCallback?.Invoke(_myServer);
        }

        public Action OnUserJoined { get; set; }

        public void SendMessage(string text, Connection? except = null)
        { 
            var data = Encoding.UTF8.GetBytes(text);
            
            foreach (var connection in _myServer.Connected)
            {
                if (connection == except) continue;
                
                connection.SendMessage(data, 0, data.Length);
            }
        }
    }

    
    public class MyServer : SocketManager
    {

        public Dictionary<ulong, string> ConnectedUsers { get; set; } = new Dictionary<ulong, string>();
        public Action OnUserJoined;
        public Action<string> MessageReceived;

        public INetworkManager Manager;
        
        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            base.OnConnected(connection, info);
            
            OnUserJoined?.Invoke();
        }
        
        public override unsafe void OnMessage( Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel )
        {
            // We're only sending strings, so it's fine to read this like this
            var str = Encoding.UTF8.GetString( (byte*)data, size );
            var steamName = SteamFriends.GetFriendPersona(identity.SteamId);
            MessageReceived?.Invoke($"{steamName}: {str}");
            
            ((SteamNetworkManager)Manager).SendMessage(str, connection);
        }
    }
    
    public class TestConnectionInterface : ConnectionManager
    {

        public Action Connected;
        public Action<string> MessageReceived;

        public override void OnConnecting(ConnectionInfo data)
        {
            Debug.WriteLine($" - OnConnecting");
        }

        /// <summary>
        /// Client is connected. They move from connecting to Connections
        /// </summary>
        public override void OnConnected(ConnectionInfo data)
        {
            Debug.WriteLine($" - OnConnected");

            Connected?.Invoke();
        }

        /// <summary>
        /// The connection has been closed remotely or disconnected locally. Check data.State for details.
        /// </summary>
        public override void OnDisconnected(ConnectionInfo data)
        {
        }

        public override unsafe void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var str = Encoding.UTF8.GetString( (byte*)data, size);
            var steamName = SteamFriends.GetFriendPersona(ConnectionInfo.Identity.SteamId);
            MessageReceived?.Invoke($"{steamName}: {str}");
        }

    }
}