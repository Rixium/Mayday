using System;
using System.Runtime.InteropServices;
using Mayday.Game.Gameplay;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class TileTypePacket : INetworkPacket
    {
        public TileType TileType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public void Dispose()
        {
            Marshal.FreeHGlobal(Data);
        }

        public IntPtr Data { get; set; }
        public int Length { get; set; }
    }
}