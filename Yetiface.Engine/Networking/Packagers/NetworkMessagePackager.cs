using System;
using System.Collections.Generic;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public class NetworkMessagePackager : INetworkMessagePackager
    {
        private Dictionary<Type, IPacketDefinition> PacketDefinitions { get; set; }

        public NetworkMessagePackager() => PacketDefinitions = new Dictionary<Type, IPacketDefinition>();

        public void AddDefinition(Type packetType, IPacketDefinition packetDefinition) => 
            PacketDefinitions.Add(packetType, packetDefinition);

        public INetworkPacket Package<T>(T value)
        {
            var packetDefinition = PacketDefinitions[value.GetType()];
            return packetDefinition.Create(value);
        }

    }
}