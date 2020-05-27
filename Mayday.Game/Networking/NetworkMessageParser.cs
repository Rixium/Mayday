using System;
using System.Text;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking
{
    public class NetworkMessageParser : INetworkMessageParser
    {
        
        public unsafe INetworkPacket Parse(IntPtr data, int size)
        {
            var stringData = Encoding.UTF8.GetString((byte*)data, size);
            
            return null;
        }
        
    }

}