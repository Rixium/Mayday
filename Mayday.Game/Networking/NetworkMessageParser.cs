using System;
using System.Text;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;

namespace Mayday.Game.Networking
{
    public class NetworkMessageParser : INetworkMessageParser
    {
        
        public unsafe INetworkMessageValue Parse(IntPtr data, int size)
        {
            var stringData = Encoding.UTF8.GetString((byte*)data, size);
            
            var splitData = stringData.Split(new[] {':'}, 3);
            
            return new Message
            {
                SteamUserId = ulong.Parse(splitData[0]),
                MessageType = (MessageType) int.Parse(splitData[1]),
                Text = splitData.Length > 2 ? splitData[3] : ""
            };
        }
        
    }

    public class Message : INetworkMessageValue
    {
        public ulong SteamUserId { get; set; }
        
        public MessageType MessageType { get; set; }
        
        public string Text;
    }
    
}