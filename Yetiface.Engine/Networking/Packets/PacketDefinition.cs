using System;
using System.Linq;

namespace Yetiface.Engine.Networking.Packets
{
    public class PacketDefinition<T> : IPacketDefinition where T : INetworkPacket, new()
    {
        public int PacketTypeId { get; set; }
        public Type PacketType { get; set; }

        public string Pack(INetworkPacket data)
        {
            var casted = (T) data;
            var propertyValues = casted.GetType().GetProperties()
                .Select(property => property.GetValue(casted).ToString()).ToList();
            return string.Join(":", propertyValues.ToArray());
        }

        public INetworkPacket Unpack(string data)
        {
            var obj = new T();
            var split = data.Split(':');
            var properties = obj.GetType().GetProperties();
            for(var i = 0; i < split.Length; i++)
            {
                var property = properties[i];
                property.SetValue(obj, Convert.ChangeType(split[i], obj.GetType().GetProperties()[i].PropertyType), null);
            }
            return obj;
        }
        
    }
}