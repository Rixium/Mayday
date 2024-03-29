using System;
using System.Collections.Generic;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;

namespace Mayday.Game.Networking.Listeners
{
    public class MaydayClientNetworkListener : INetworkClientListener
    {
        private readonly INetworkMessagePackager _messagePackager;

        private readonly Dictionary<Type, IPacketConsumer> _packetConsumers =
            new Dictionary<Type, IPacketConsumer>();

        public MaydayClientNetworkListener(INetworkManager networkManager)
        {
            _messagePackager = networkManager.MessagePackager;
            networkManager.SetClientNetworkListener(this);
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var received = _messagePackager.Unpack(data, size);
            var receivedType = received.GetType();

            if (!_packetConsumers.ContainsKey(receivedType))
                return;

            var consumer = _packetConsumers[receivedType];
            consumer.Consume(received);
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
        }

        public void AddConsumer(IPacketConsumer packetConsumer) =>
            _packetConsumers.Add(packetConsumer.PacketType, packetConsumer);
    }
}