using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yetiface.Engine.Networking.Packets
{
    public class PacketDefinition<T> : IPacketDefinition where T : INetworkPacket, new()
    {
        public int PacketTypeId { get; set; }
        public Type PacketType { get; set; }

        public string Pack(INetworkPacket data)
        {
            var casted = (T) data;
            var properties = casted.GetType().GetProperties();
            var listOfValues = new List<string>();
            foreach (var property in properties)
            {
                var value = property.GetValue(casted);
                if (value.GetType().IsEnum)
                {
                    var asInt = (int) value;
                    listOfValues.Add(asInt.ToString());
                    continue;
                }

                listOfValues.Add(value.ToString());
            }
            
            return string.Join(":", listOfValues.ToArray());
        }

        public INetworkPacket Unpack(string data)
        {
            var obj = new T();
            var split = data.Split(':');
            var properties = obj.GetType().GetProperties();
            
            for(var i = 0; i < split.Length; i++)
            {
                var property = properties[i];
                property.SetValue(obj, CastPropertyValue(property, split[i]), null);
            }
            
            return obj;
        }

        private static object CastPropertyValue(PropertyInfo property, string value) { 
            if (property == null || string.IsNullOrEmpty(value))
                return null;
            
            if (property.PropertyType.IsEnum)
            {
                var enumType = property.PropertyType;
                return Enum.Parse(enumType, value);
            }
            
            if (property.PropertyType == typeof(bool))
                return value == "1" || value == "true" || value == "on" || value == "checked";
            
            return property.PropertyType == typeof(Uri) ? 
                new Uri(Convert.ToString(value)) : 
                Convert.ChangeType(value, property.PropertyType);
        }
        
    }
}