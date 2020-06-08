using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.SteamNetworking
{
    public class SteamNetworkManager : INetworkManager
    {
        private readonly uint _appId;

        public INetworkServerListener NetworkServerListener { get; set; }
        public INetworkClientListener NetworkClientListener { get; set; }
        public INetworkMessagePackager MessagePackager { get; set; }

        private List<Connection> Connections { get; set; } = new List<Connection>();

        public SteamNetworkManager(uint appId, INetworkMessagePackager messagePackager)
        {
            _appId = appId;
            MessagePackager = messagePackager;
        }

        public ConnectionManager Client { get; set; }
        public SocketManager Server { get; set; }

        public SocketManager CreateSession(string port)
        {
            var netAddress = NetAddress.AnyIp(ushort.Parse(port));
            Server = SteamNetworkingSockets.CreateNormalSocket<MaydayServer>(netAddress);

            ((MaydayServer) Server).NetworkManager = this;

            SteamMatchmaking.OnLobbyCreated += (result, lobby) => OnLobbyCreated(result, lobby, port);
            SteamMatchmaking.CreateLobbyAsync();

            return Server;
        }

        private void OnLobbyCreated(Result _, Lobby lobby, string port)
        {
            lobby.SetPublic();
            lobby.SetData("ip", new WebClient().DownloadString("http://ipv4.icanhazip.com/"));
            lobby.SetData("port", port);
        }

        public ConnectionManager JoinSession(string ip, string port)
        {
            Client = SteamNetworkingSockets.ConnectNormal<MaydayClient>(NetAddress.From(ip, ushort.Parse(port)));
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
        
        public void SendMessage(byte[] data, Connection to) => to.SendMessage(data);

        public void RelayMessage(IntPtr data, int size, Connection ignore)
        {
            if (Server?.Connected == null) return;
            
            foreach (var connection in Server.Connected.Where(connection => connection != ignore))
                connection.SendMessage(data, size);
        }
    }
}