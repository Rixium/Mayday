﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
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

            SteamMatchmaking.OnLobbyCreated += (result, lobby) => OnLobbyCreated(result, lobby);
            SteamMatchmaking.CreateLobbyAsync();
            
            return Server;
        }

        private void OnLobbyCreated(Result _, Lobby lobby)
        {
            lobby.SetPublic();
            lobby.SetData("ip", new WebClient().DownloadString("http://icanhazip.com"));
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

        public void SetServerNetworkListener(INetworkServerListener networkServerListener) => NetworkServerListener = networkServerListener;

        public void SetClientNetworkListener(INetworkClientListener clientNetworkListener) => NetworkClientListener = clientNetworkListener;
        
        public void SendMessage(string value)
        {
            var toSend = $"{SteamClient.SteamId}:{value}";
            
            if (Server?.Connected != null)
                foreach (var connection in Server.Connected)
                {
                    connection.SendMessage(toSend);
                }

            Client?.Connection.SendMessage(toSend);
        }
        
        private IPAddress GetExternalIPAddress()
        {
            var myIPHostEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var myIPAddress in myIPHostEntry.AddressList)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();

                if( myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
                {
                    return myIPAddress;
                }
            }

            return null;
        }
    }
}