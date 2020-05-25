using System;

namespace Yetiface.Engine.Networking
{
    public interface INetworkMessageParser
    {

        INetworkMessageValue Parse(IntPtr data, int size);
        
    }
}