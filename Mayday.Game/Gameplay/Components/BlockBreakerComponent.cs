using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.Networking;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Gameplay.Components
{
    public class BlockBreakerComponent : IComponent
    {
        private readonly Camera _camera;

        private readonly INetworkManager _networkManager;
        private readonly MaydayMessagePackager _messagePackager;
        
        public IPlayer Player { get; set; }
        public IGameWorld GameWorld { get; set; }

        public int MaxDistanceToBreak { get; set; } = 30;
        
        public BlockBreakerComponent(IGameWorld gameWorld, Camera camera, INetworkManager networkManager)
        {
            GameWorld = gameWorld;
            _camera = camera;
            
            _networkManager = networkManager;
            _messagePackager = new MaydayMessagePackager();
        }
        
        public void Update()
        {
            if (MouseState.CurrentState.LeftButton == ButtonState.Pressed)
            {
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());

                var mouseTileX = mousePosition.X / GameWorld.TileSize;
                var mouseTileY = mousePosition.Y / GameWorld.TileSize;
                
                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > GameWorld.Width - 1 ||
                    mouseTileY > GameWorld.Height - 1) return;
                var tile = GameWorld.Tiles[mouseTileX, mouseTileY];
                
                if (DistanceFromBlock(tile) > MaxDistanceToBreak) return;

                var oldType = tile.TileType;
                tile.TileType = 1;

                if (oldType != tile.TileType)
                {
                    SendTileChangePacket(tile);
                }
                
            } else if (MouseState.CurrentState.RightButton == ButtonState.Pressed)
            {
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());

                var mouseTileX = mousePosition.X / GameWorld.TileSize;
                var mouseTileY = mousePosition.Y / GameWorld.TileSize;
                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > GameWorld.Width - 1 ||
                    mouseTileY > GameWorld.Height - 1) return;
                
                var tile = GameWorld.Tiles[mouseTileX, mouseTileY];

                if (DistanceFromBlock(tile) > MaxDistanceToBreak) return;

                var oldType = tile.TileType;
                tile.TileType = 0;

                if (oldType != tile.TileType)
                {
                    SendTileChangePacket(tile);
                }
            }

        }

        private float DistanceFromBlock(Tile tile)
        {
            return Vector2.Distance(tile.RenderCenter, Player.Center);
        }

        private void SendTileChangePacket(Tile tile)
        {
            var tileChangePacket = new TileTypePacket()
            {
                X = tile.X,
                Y = tile.Y,
                TileType = tile.TileType
            };

            var package = _messagePackager.Package(tileChangePacket);
            
            _networkManager.SendMessage(package);
        }
        
        public void OnAddedToPlayer()
        {
            
        }
    }
}