using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;

namespace Yetiface.Engine.Networking.SteamNetworking
{
    public class SteamNetworkManager : INetworkManager
    {
        private readonly uint _appId;
        
        public INetworkServerListener NetworkServerListener;
        public INetworkClientListener NetworkClientListener;

        private List<Connection> Connections { get; set; } = new List<Connection>();

        public SteamNetworkManager(uint appId)
        {
            _appId = appId;
        }

        public ConnectionManager Client { get; set; }
        public SocketManager Server { get; set; }

        public SocketManager CreateSession()
        {
            Server = SteamNetworkingSockets.CreateNormalSocket<MaydayServer>(NetAddress.AnyIp(25565));
            ((MaydayServer) Server).NetworkManager = this;
            
            return Server;
        }

        public ConnectionManager JoinSession(string ip)
        {
            Client = SteamNetworkingSockets.ConnectNormal<MaydayClient>(NetAddress.From(ip, 25565)) as MaydayClient;
            ((MaydayClient) Client).NetworkManager = this;
            
            return Client;
        }

        public void Update()
        {
            Server?.Receive();
            Client?.Receive();
        }

        public void SetServerNetworkListener(INetworkServerListener networkServerListener) => NetworkServerListener = networkServerListener;

        public void SetClientNetworkListener(INetworkClientListener clientNetworkListener) => NetworkClientListener = clientNetworkListener;
        public void SendMessage(string value)
        {
            if (Server?.Connected != null)
                foreach (var connection in Server.Connected)
                {
                    connection.SendMessage(value);
                }

            Client?.Connection.SendMessage(value);
        }
    }
}