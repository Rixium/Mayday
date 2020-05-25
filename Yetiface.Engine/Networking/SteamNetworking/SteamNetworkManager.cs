using System.Collections.Generic;
using System.Net;
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

        public void SendMessage(MessageType messageType, string value = "")
        {
            var toSend = $"{SteamClient.SteamId}:{(int) messageType}";

            // If the value is empty, then don't add it to the to send,
            // otherwise, add it to the it.
            toSend = value.Equals("") ? $"{toSend}" : $"{toSend}:{value}";

            if (Server?.Connected != null)
                foreach (var connection in Server.Connected)
                {
                    connection.SendMessage(toSend);
                }

            Client?.Connection.SendMessage(toSend);
        }
    }

    public enum MessageType
    {
        ChatMessage,
        WorldRequest,
        WorldSendComplete
    }
}