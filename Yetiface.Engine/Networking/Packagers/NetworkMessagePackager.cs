using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking.Packagers
{
    public class NetworkMessagePackager : INetworkMessagePackager
    {
        private readonly Dictionary<Type, IPacketDefinition> _packetDefinitions;

        public NetworkMessagePackager() => _packetDefinitions = new Dictionary<Type, IPacketDefinition>();

        public void AddDefinition(Type packetType, IPacketDefinition packetDefinition)
        {
            packetDefinition.PacketType = packetType;
            _packetDefinitions.Add(packetType, packetDefinition);
        }

        public byte[] Package<T>(T value) where T : INetworkPacket
        {
            var packetDefinition = _packetDefinitions[value.GetType()];
            var asString = packetDefinition.Pack(value);
            return Encoding.UTF8.GetBytes($"{packetDefinition.PacketTypeId}:{asString}");
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

        public IPacketDefinition GetPacketDefinition(int packetTypeId) =>
            _packetDefinitions.FirstOrDefault(packetDefinition => packetDefinition.Value.PacketTypeId == packetTypeId)
                .Value;
        
    }
}