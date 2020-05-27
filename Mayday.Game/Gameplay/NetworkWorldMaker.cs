using System;
using System.Drawing;
using System.Threading.Tasks;
using Mayday.Game.Networking;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;
using Color = Microsoft.Xna.Framework.Color;

namespace Mayday.Game.Gameplay
{
    public class NetworkWorldMaker : IWorldMaker, INetworkClientListener
    {
        private readonly INetworkManager _networkManager;
        private bool _worldRequesting;
        private int _tilesReceived;
        private int _worldWidth;
        private int _worldHeight;
        private Tile[,] _tiles;
        private bool _gotWorld;

        INetworkMessageParser _messageParser = new NetworkMessageParser();
        
        public int WorldSize { get; set; }
        public Bitmap Bitmap { get; set; }

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
            // Once we sent the message, we can run a task to check for how long it's going to take
            return await GetWorldFromNetwork(listener);
        }

        private async Task<IWorld> GetWorldFromNetwork(IWorldGeneratorListener worldGeneratorListener)
        {
            var world = new World();

            _worldRequesting = true;
            
            await Task.Delay(1000);
            
            _tilesReceived = 0;
            _worldWidth = 200;
            _worldHeight = 200;
            _tiles = new Tile[_worldWidth, _worldHeight];

            for (var i = 0; i < _worldWidth; i++)
            {
                for (var j = 0; j < _worldHeight; j++)
                {
                    _tiles[i, j] = new Tile(TileType.NONE, i, j);
                }
            }
            
            Bitmap = new Bitmap(_worldWidth, _worldHeight);

            // Send the first request to get all the tiles from the host.
            _networkManager.SendMessage(MessageType.WorldRequest);

            while (_tilesReceived < _worldWidth * _worldHeight)
            {
                var percent = ((float)_tilesReceived / (_worldWidth * _worldHeight)) * 100;
                worldGeneratorListener.OnWorldGenerationUpdate($"Receiving tiles... {percent}%");
            }
            
            worldGeneratorListener.OnWorldGenerationUpdate("Got tiles...");

            foreach (var tile in _tiles)
            {
                Bitmap.SetPixel(tile.X, tile.Y, tile.TileType == TileType.NONE ? System.Drawing.Color.Black :
                    tile.TileType == TileType.GROUND ? System.Drawing.Color.White : System.Drawing.Color.Orange);
            }

            world.Tiles = _tiles;
            
            return world;
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var message = _messageParser.Parse(data, size);

            switch (message.MessageType)
            {
                case MessageType.ChatMessage:
                    break;
                case MessageType.WorldRequest:
                    Console.WriteLine("WORLD REQUEST!");
                    break;
                case MessageType.WorldSendComplete:
                    Console.WriteLine("Received World!");
                    break;
                case MessageType.TileData:
                    _tilesReceived++;
                    var tileData = (TileData) message;
                    _tiles[tileData.X, tileData.Y].TileType = tileData.TileType;
                    break;
            }
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }
        
    }
}