using System;
using System.Collections.Generic;
using Yetiface.Engine.Networking.SteamNetworking;

namespace Yetiface.Engine.Networking
{
    /// <summary>
    /// Allows for a common abstraction for all types of network management. Be it steam or peer-to-peer by IP.
    /// </summary>
    public interface INetworkManager
    {
        Dictionary<ulong, string> ConnectedUsers { get; }
        
        void CreateSession(Action<MyServer> lobbyCreatedCallback);
        Action OnUserJoined { get; set; }
    }
}