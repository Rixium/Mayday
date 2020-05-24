using System;

namespace Yetiface.Engine.Networking
{
    /// <summary>
    /// Allows for a common abstraction for all types of network management. Be it steam or peer-to-peer by IP.
    /// </summary>
    public interface INetworkManager
    {
        void CreateSession(Action<ILobbyInformation> lobbyCreatedCallback);
    }
}