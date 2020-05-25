using System;
using System.Text;
using Yetiface.Engine.Networking;

namespace Mayday.Game.Networking
{
    public class NetworkMessageParser : INetworkMessageParser
    {
        
        public unsafe INetworkMessageValue Parse(IntPtr data, int size)
        {
            var stringData = Encoding.UTF8.GetString((byte*)data, size);
            
            var splitData = stringData.Split(new[] {':'}, 2);
            
            return new Message
            {
                SteamUserId = ulong.Parse(splitData[0]),
                Text = splitData[1]
            };
        }
        
    }

    public class Message : INetworkMessageValue
    {
        public ulong SteamUserId { get; set; }
        public string Text;
    }
    
}