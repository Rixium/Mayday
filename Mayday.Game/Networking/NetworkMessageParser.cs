using System;
using System.Text;
using Mayday.Game.Gameplay;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;

namespace Mayday.Game.Networking
{
    public class NetworkMessageParser : INetworkMessageParser
    {
        
        public unsafe INetworkMessageValue Parse(IntPtr data, int size)
        {
            var stringData = Encoding.UTF8.GetString((byte*)data, size);
            
            var splitData = stringData.Split(new[] {':'}, 1000);

            var messageType = (MessageType) int.Parse(splitData[1]);

            switch (messageType)
            {
                case MessageType.ChatMessage:
                    return new Message
                    {
                        SteamUserId = ulong.Parse(splitData[0]),
                        MessageType = (MessageType) int.Parse(splitData[1]),
                        Text = splitData.Length > 2 ? splitData[3] : ""
                    };
                case MessageType.WorldRequest:
                    return new Message
                    {
                        SteamUserId = ulong.Parse(splitData[0]),
                        MessageType = (MessageType) int.Parse(splitData[1]),
                        Text = splitData.Length > 2 ? splitData[3] : ""
                    };
                case MessageType.WorldSendComplete:
                    return new Message
                    {
                        SteamUserId = ulong.Parse(splitData[0]),
                        MessageType = (MessageType) int.Parse(splitData[1]),
                        Text = splitData.Length > 2 ? splitData[3] : ""
                    };
                case MessageType.TileData:
                    return new TileData()
                    {
                        SteamUserId = ulong.Parse(splitData[0]),
                        MessageType = (MessageType) int.Parse(splitData[1]),
                        X = int.Parse(splitData[2]),
                        Y = int.Parse(splitData[3]),
                        TileType = (TileType) int.Parse(splitData[4])
                    };
                case MessageType.TileReceived:
                    return new Message
                    {
                        SteamUserId = ulong.Parse(splitData[0]),
                        MessageType = (MessageType) int.Parse(splitData[1]),
                        Text = splitData.Length > 2 ? splitData[3] : ""
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }

    public class Message : INetworkMessageValue
    {
        public ulong SteamUserId { get; set; }
        
        public MessageType MessageType { get; set; }
        
        public string Text;
    }
    
    public class TileData : INetworkMessageValue
    {
        public ulong SteamUserId { get; set; }
        
        public MessageType MessageType { get; set; }
        
        public int X { get; set; }
        
        public int Y { get; set; }
        
        public TileType TileType { get; set; }
        
    }
    
}