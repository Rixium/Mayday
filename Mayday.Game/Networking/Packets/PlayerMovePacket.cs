﻿using Yetiface.Engine.Networking.Packets;

namespace Mayday.Game.Networking.Packets
{
    public class PlayerMovePacket : INetworkPacket
    {
        
        public uint SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
    }
}