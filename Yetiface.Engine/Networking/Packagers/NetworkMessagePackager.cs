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
        private readonly Dictionary<int, IPacketDefinition> _packetDefinitions;

        public NetworkMessagePackager() => _packetDefinitions = new Dictionary<int, IPacketDefinition>();

        public IPacketDefinition AddDefinition<T>() where T : INetworkPacket, new()
        {
            var packetDefinition = new PacketDefinition<T>()
            {
                PacketType = typeof(T)
            };
            
            _packetDefinitions.Add(0, packetDefinition);

            return packetDefinition;
        }

        public byte[] Package<T>(T value) where T : INetworkPacket
        {
            var packetDefinition = _packetDefinitions[0];
            var asString = packetDefinition.Pack(value);
            return Encoding.UTF8.GetBytes($"{packetDefinition.PacketTypeId}:{asString}");
        }

        public INetworkPacket Unpack(IntPtr data, int size)
        {
            var output = Marshal.PtrToStringAuto(data);
            if (output == null)
                throw new Exception("Could not convert data to string.");

            // Split in to 2 strings because we know the first is going to be the packet type id.
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

            // Split in to 2 strings because we know the first is going to be the packet type id.
            var split = output.Split(new[] {':'}, 2);
            var packetTypeId = int.Parse(split[0]);
            var packetDefinition = GetPacketDefinition(packetTypeId);
            return packetDefinition.Unpack(split[1]);
        }

        /// <summary>
        /// Retrieves the packet definition from the dictionary depending on the packet type id that has been passed.
        /// </summary>
        /// <param name="packetTypeId">The packet type Id of the expected packet definition.</param>
        /// <returns>The packet definition for the given packet type id.</returns>
        ///  TODO Hate this, probably slow. If a lot of packets are flying around and we're doing dictionary look ups using LINQ every time..
        public IPacketDefinition GetPacketDefinition(int packetTypeId) =>
            _packetDefinitions.FirstOrDefault(packetDefinition => packetDefinition.Value.PacketTypeId == packetTypeId)
                .Value;
        
    }
}