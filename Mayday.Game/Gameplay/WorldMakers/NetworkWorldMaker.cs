﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;
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
        private readonly NetworkMessagePackager _networkMessagePackager;
        
        private int _tilesReceived;
        
        public int WorldWidth { get; set; }
        
        public int WorldHeight { get; set; }

        private Tile[,] _tiles;

        private int WorldSize { get; set; }
        private Bitmap Bitmap { get; set; }

        public IWorldMaker SetWorldSize(int worldSize)
        {
            WorldSize = worldSize;
            return this;
        }

        public NetworkWorldMaker(INetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.SetClientNetworkListener(this);
            _networkMessagePackager = new MaydayMessagePackager();
        }

        public async Task<IGameWorld> Create(IWorldMakerListener listener)
        {
            // Once we sent the message, we can run a task to check for how long it's going to take
            return await GetWorldFromNetwork(listener);
        }

        private async Task<IGameWorld> GetWorldFromNetwork(IWorldMakerListener worldGeneratorListener)
        {
            var world = new GameWorld();

            await Task.Delay(1000);
            
            _tilesReceived = 0;
            
            WorldWidth = 200;
            WorldHeight = 200;
            
            _tiles = new Tile[WorldWidth, WorldHeight];

            for (var i = 0; i < WorldWidth; i++)
            {
                for (var j = 0; j < WorldHeight; j++)
                {
                    _tiles[i, j] = new Tile(TileType.NONE, i, j)
                    {
                        GameWorld = world
                    };
                }
            }
            
            Bitmap = new Bitmap(WorldWidth, WorldHeight);
            
            var mapRequest = new MapRequestPacket();
            var toSend = _networkMessagePackager.Package(mapRequest);
            _networkManager.SendMessage(toSend);
            
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
            world.TileSize = 12;
            
            return world;
        }

        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var packet = _networkMessagePackager.Unpack(data, size);
            
            if (packet.GetType() == typeof(TileTypePacket))
            {
                var tileTypePacket = (TileTypePacket) packet;
                var x = tileTypePacket.X;
                var y = tileTypePacket.Y;
                var tileType = tileTypePacket.TileType;
                
                _tiles[x, y].TileType = tileType;
            }
            
            _tilesReceived++;
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }
        
    }
}