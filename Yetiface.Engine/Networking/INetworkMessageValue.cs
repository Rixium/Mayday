﻿using Yetiface.Engine.Networking.SteamNetworking;

 namespace Yetiface.Engine.Networking
{
    public interface INetworkMessageValue
    {
       
        ulong SteamUserId { get; set; }
        MessageType MessageType { get; set; }
        
    }
}