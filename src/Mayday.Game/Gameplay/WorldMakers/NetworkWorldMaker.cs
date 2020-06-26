using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.World.Areas;
using Mayday.Game.Gameplay.WorldMakers.Listeners;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Consumers;
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
        
        public int AreaWidth { get; set; }
        
        public int AreaHeight { get; set; }

        private Tile[,] _tiles;
        public IList<NewPlayerPacket> PlayersToAdd = new List<NewPlayerPacket>();

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

            await Task.Delay(1000);
            
            _tilesReceived = 0;
            
            AreaWidth = 200;
            AreaHeight = 200;
            
            _tiles = new Tile[AreaWidth, AreaHeight];

            var outsideArea = new OutsideArea()
            {
                Tiles = _tiles,
                AreaWidth = AreaWidth,
                AreaHeight = AreaHeight,
            };

            var world = new GameWorld(outsideArea)
            {
                TileSize = 8 * Game1.GlobalGameScale
            };

            for (var i = 0; i < AreaWidth; i++)
            {
                for (var j = 0; j < AreaHeight; j++)
                {
                    _tiles[i, j] = new Tile(TileTypes.None, i, j)
                    {
                        GameWorld = world,
                        GameArea = outsideArea
                    };
                }
            }
            
            Bitmap = new Bitmap(AreaWidth, AreaHeight);
            
            var mapRequest = new MapRequestPacket();
            var toSend = _networkMessagePackager.Package(mapRequest);
            _networkManager.SendMessage(toSend);
            
            while (_tilesReceived < AreaWidth * AreaHeight)
            {
                var percent = ((float)_tilesReceived / (AreaWidth * AreaHeight)) * 100;
                worldGeneratorListener.OnWorldGenerationUpdate($"Receiving tiles... {percent}%");
            }
            
            worldGeneratorListener.OnWorldGenerationUpdate("Got tiles...");
            
            while (!ReceivedMap)
            {
                worldGeneratorListener.OnWorldGenerationUpdate("Waiting for players...");
            }
            
            foreach (var tile in _tiles)
            {
                Bitmap.SetPixel(tile.TileX, tile.TileY, tile.TileType == TileTypes.None ? Color.Black :
                    tile.TileType == TileTypes.Dirt ? Color.White : Color.Orange);
            }

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
            } else if (packet.GetType() == typeof(NewPlayerPacket))
            {
                var newPlayerPacket = (NewPlayerPacket) packet;
                PlayersToAdd.Add(newPlayerPacket);
            } else if (packet.GetType() == typeof(MapSendCompletePacket))
            {
                ReceivedMap = true;
            }
            
            _tilesReceived++;
        }

        public bool ReceivedMap { get; set; }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }

        public void AddConsumer(IPacketConsumer packetConsumer)
        {
            
        }
    }
}