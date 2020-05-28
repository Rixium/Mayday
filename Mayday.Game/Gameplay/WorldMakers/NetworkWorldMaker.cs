using System;
using System.Drawing;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Color = System.Drawing.Color;

namespace Mayday.Game.Gameplay.WorldMakers
{
    
    /// <summary>
    /// The network world maker will just instead of making a map manually, will request the tiles from a
    /// host using a network manager.
    /// </summary>
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

        public async Task<IGameWorld> Create(IWorldMakerListener listener)
        {
            // Once we sent the message, we can run a task to check for how long it's going to take
            return await GetWorldFromNetwork(listener);
        }

        private async Task<IGameWorld> GetWorldFromNetwork(IWorldMakerListener worldGeneratorListener)
        {
            var world = new GameWorld();

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