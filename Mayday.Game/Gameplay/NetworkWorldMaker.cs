using System;
using System.Drawing;
using System.Threading.Tasks;
using Mayday.Game.Networking;
using Mayday.Game.Networking.SteamNetworking;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Color = System.Drawing.Color;

namespace Mayday.Game.Gameplay
{
    public class NetworkWorldMaker : IWorldMaker, INetworkClientListener
    {
        private readonly INetworkManager _networkManager;
        private bool _worldRequesting;
        private int _tilesReceived;
        
        public int WorldWidth { get; set; }
        
        public int WorldHeight { get; set; }

        private Tile[,] _tiles;
        private bool _gotWorld;

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
            WorldWidth = 200;
            WorldHeight = 200;
            _tiles = new Tile[WorldWidth, WorldHeight];

            for (var i = 0; i < WorldWidth; i++)
            {
                for (var j = 0; j < WorldHeight; j++)
                {
                    _tiles[i, j] = new Tile(TileType.NONE, i, j);
                }
            }
            
            Bitmap = new Bitmap(WorldWidth, WorldHeight);

            while (_tilesReceived < WorldWidth * WorldHeight)
            {
                var percent = ((float)_tilesReceived / (WorldWidth * WorldHeight)) * 100;
                worldGeneratorListener.OnWorldGenerationUpdate($"Receiving tiles... {percent}%");
            }
            
            worldGeneratorListener.OnWorldGenerationUpdate("Got tiles...");

            foreach (var tile in _tiles)
            {
                Bitmap.SetPixel(tile.X, tile.Y, tile.TileType == TileType.NONE ? Color.Black :
                    tile.TileType == TileType.GROUND ? Color.White : Color.Orange);
            }

            world.Tiles = _tiles;
            world.Width = WorldWidth;
            world.Height = WorldHeight;
            
            return world;
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }
        
    }
}