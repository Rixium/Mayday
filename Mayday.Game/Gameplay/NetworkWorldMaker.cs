using System;
using System.Threading.Tasks;
using Mayday.Game.Networking;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;

namespace Mayday.Game.Gameplay
{
    public class NetworkWorldMaker : IWorldMaker, INetworkClientListener
    {
        private readonly INetworkManager _networkManager;
        private bool _worldRequesting;

        public int WorldSize { get; set; }
        
        public IWorldMaker SetWorldSize(int worldSize)
        {
            WorldSize = worldSize;
            return this;
        }

        public NetworkWorldMaker(INetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.SetClientNetworkListener(this);
        }
        
        public async Task<IWorld> Create(IWorldGeneratorListener listener)
        {
            // Send the first request to get all the tiles from the host.
            _networkManager.SendMessage(MessageType.WorldRequest);

            // Once we sent the message, we can run a task to check for how long it's going to take
            return await GetWorldFromNetwork(listener);
        }

        private async Task<IWorld> GetWorldFromNetwork(IWorldGeneratorListener worldGeneratorListener)
        {
            var world = new World();

            _worldRequesting = true;
            
            await Task.Delay(1000);

            while (_worldRequesting)
            {
                worldGeneratorListener.OnWorldGenerationUpdate("Getting tiles...");
            }
            
            worldGeneratorListener.OnWorldGenerationUpdate("Got tiles...");

            return world;
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var message = new NetworkMessageParser().Parse(data, size);

            switch (message.MessageType)
            {
                case MessageType.ChatMessage:
                    break;
                case MessageType.WorldRequest:
                    Console.WriteLine("WORLD REQUEST!");
                    break;
                case MessageType.WorldSendComplete:
                    Console.WriteLine("Received World!");
                    _worldRequesting = false;
                    break;
            }
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }
        
    }
}