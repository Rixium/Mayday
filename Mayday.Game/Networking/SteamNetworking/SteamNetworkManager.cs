using System.Collections.Generic;
using System.Net;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;

namespace Mayday.Game.Networking.SteamNetworking
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
            var netAddress = NetAddress.AnyIp(25565);
            Server = SteamNetworkingSockets.CreateNormalSocket<MaydayServer>(netAddress);

            ((MaydayServer) Server).NetworkManager = this;

            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.CreateLobbyAsync();

            return Server;
        }

        private void OnLobbyCreated(Result _, Lobby lobby)
        {
            lobby.SetPublic();
            lobby.SetData("ip", new WebClient().DownloadString("http://ipv4.icanhazip.com/"));
        }

        public ConnectionManager JoinSession(string ip)
        {
            Client = SteamNetworkingSockets.ConnectNormal<MaydayClient>(NetAddress.From(ip, 25565));
            ((MaydayClient) Client).NetworkManager = this;

            return Client;
        }

        public void Update()
        {
            Server?.Receive();
            Client?.Receive();
        }

        public void SetServerNetworkListener(INetworkServerListener networkServerListener) =>
            NetworkServerListener = networkServerListener;

        public void SetClientNetworkListener(INetworkClientListener clientNetworkListener) =>
            NetworkClientListener = clientNetworkListener;

        public void SendMessage(byte[] data)
        {
            if (Server?.Connected != null)
                foreach (var connection in Server.Connected)
                {
                    connection.SendMessage(data);
                }

            Client?.Connection.SendMessage(data);
        }
        
        public void SendMessage(byte[] data, Connection to) => 
            to.SendMessage(data);
        
    }
}