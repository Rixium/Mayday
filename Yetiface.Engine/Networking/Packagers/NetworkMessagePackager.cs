using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public class NetworkMessagePackager : INetworkMessagePackager
    {
        private readonly Dictionary<int, IPacketDefinition> _packetDefinitions;

        public NetworkMessagePackager() => _packetDefinitions = new Dictionary<int, IPacketDefinition>();

        public void AddDefinition(IPacketDefinition packetDefinition) =>
            _packetDefinitions.Add(packetDefinition.PacketTypeId, packetDefinition);

        public byte[] Package<T>(T value) where T : INetworkPacket
        {
            var packetDefinition = _packetDefinitions[value.PacketTypeId];
            var asString = packetDefinition.Create(value);
            return Encoding.UTF8.GetBytes($"{value.PacketTypeId}:{asString}");
        }

        public INetworkPacket Unpack(IntPtr data, int size)
        {
            var output = Marshal.PtrToStringAuto(data);
            if (output == null)
                throw new Exception("Could not convert data to string.");

            var split = output.Split(new[] {':'}, 2);
            var packetTypeId = int.Parse(split[0]);
            var packetDefinition = GetPacketDefinition(packetTypeId);
            return packetDefinition.Unpack(split[1]);
        }

        public INetworkPacket Unpack(byte[] data)
        {
            var output = Encoding.UTF8.GetString(data);

            if (output == null)
                throw new Exception("Could not convert data to string.");

            var split = output.Split(new[] {':'}, 2);
            var packetTypeId = int.Parse(split[0]);
            var packetDefinition = GetPacketDefinition(packetTypeId);
            return packetDefinition.Unpack(split[1]);
        }

        public IPacketDefinition GetPacketDefinition(int packetTypeId)
        {
            var packetDefinition = _packetDefinitions[packetTypeId];
            return packetDefinition;
        }
    }
}