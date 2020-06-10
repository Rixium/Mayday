using System;
using Yetiface.Engine.Networking.Packets;

namespace Yetiface.Engine.Networking
{
    public interface INetworkMessageParser
    {
        INetworkPacket Parse(IntPtr data, int size);
        
    }
}