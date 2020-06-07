using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework.Input;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Gameplay.Components
{
    public class BlockBreakerComponent : IComponent
    {
        private readonly Camera _camera;

        public IEntity Entity { get; set; }
        public IGameWorld GameWorld { get; set; }

        public int MaxDistanceToBreak => 7 * GameWorld.TileSize;
        
        public BlockBreakerComponent(IGameWorld gameWorld, Camera camera)
        {
            GameWorld = gameWorld;
            _camera = camera;
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

                if (!CloseEnoughToTile(tile)) return;
                GameWorld.PlaceTile(tile, 1);
                
            } else if (MouseState.CurrentState.RightButton == ButtonState.Pressed)
            {
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());

                var mouseTileX = mousePosition.X / GameWorld.TileSize;
                var mouseTileY = mousePosition.Y / GameWorld.TileSize;
                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > GameWorld.Width - 1 ||
                    mouseTileY > GameWorld.Height - 1) return;
                
                var tile = GameWorld.Tiles[mouseTileX, mouseTileY];

                if (!CloseEnoughToTile(tile)) return;
                tile.Destroy();
            }

        }

        private bool CloseEnoughToTile(Tile tile)
        {
            var playerBounds = Entity.GetBounds();
            var playerLeft = playerBounds.Left;
            var playerRight = playerBounds.Right;
            var playerTop = playerBounds.Top;
            var playerBottom = playerBounds.Bottom;
            
            if (Math.Abs(tile.RenderCenter.X - playerLeft) > MaxDistanceToBreak) return false;
            if (Math.Abs(tile.RenderCenter.X - playerRight) > MaxDistanceToBreak) return false;
            if (Math.Abs(tile.RenderCenter.Y - playerTop) > MaxDistanceToBreak) return false;
            return (Math.Abs(tile.RenderCenter.Y - playerBottom) <= MaxDistanceToBreak);
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}