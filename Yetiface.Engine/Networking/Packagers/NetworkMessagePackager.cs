using System;
using System.Collections.Generic;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public class NetworkMessagePackager : INetworkMessagePackager
    {
        public Dictionary<Type, IPacketDefinition> PacketDefinitions { get; }

        public NetworkMessagePackager() => PacketDefinitions = new Dictionary<Type, IPacketDefinition>();

        public void AddDefinition(Type packetType, IPacketDefinition packetDefinition) => 
            PacketDefinitions.Add(packetType, packetDefinition);

        public byte[] Package<T>(T value) where T : INetworkPacket
        {
            var packetDefinition = PacketDefinitions[value.GetType()];
            return packetDefinition.Create(value);
        }

        public IPacketDefinition GetPacketDefinition(Type type)
        {
            var packetDefinition = PacketDefinitions[type];
            return packetDefinition;
        }
    }
}