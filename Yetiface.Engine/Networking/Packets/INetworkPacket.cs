using System;

namespace Yetiface.Engine.Networking.Packets
{
    public interface INetworkPacket
    {
        void Dispose();

        IntPtr Data { get; set; }
        int Length { get; set; }
    }
    
}