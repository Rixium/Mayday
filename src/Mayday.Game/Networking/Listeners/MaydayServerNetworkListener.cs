using System;
using System.Collections.Generic;
using Mayday.Game.Networking.SteamNetworking;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.Listeners
{
    public class MaydayServerNetworkListener : INetworkServerListener
    {
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;

        private readonly Dictionary<Type, IPacketConsumer> _packetConsumers 
            = new Dictionary<Type, IPacketConsumer>();

        public MaydayServerNetworkListener(INetworkManager networkManager)
        {
            _networkManager = networkManager;
            _messagePackager = networkManager.MessagePackager;
            _networkManager.SetServerNetworkListener(this);
        }
        
        public void OnNewConnection(Connection connection, ConnectionInfo info)
        {
            
        }

        public void OnConnectionLeft(Connection connection, ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
            long recvTime, int channel)
        {
            var received = _messagePackager.Unpack(data, size);
            var receivedType = received.GetType();
            
            if (!_packetConsumers.ContainsKey(receivedType)) 
                return;
            
            var consumer = _packetConsumers[receivedType];
            consumer.Consume(connection, received);
            
            _networkManager.RelayMessage(data, size, connection);
        }

        public void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            
        }
        
        public void AddConsumer(IPacketConsumer packetConsumer) => 
            _packetConsumers.Add(packetConsumer.PacketType, packetConsumer);
        
    }
}