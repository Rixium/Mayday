using System;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;

namespace Mayday.Game.Gameplay.Items.Placers
{
    public class TilePlacer : IPlacer
    {
        private static int MaxDistanceToPlace => 7;

        public bool PlacerFor(IItem item) => !string.IsNullOrWhiteSpace(item.TileType);

        public bool Place(IEntity entity, IGameWorld gameWorld, IItem selectedItem, int mouseTileX, int mouseTileY)
        {
            if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > entity.GameArea.AreaWidth - 1 ||
                mouseTileY > entity.GameArea.AreaHeight - 1) return false;

            if (!CanPlaceAt(entity, gameWorld, mouseTileX, mouseTileY, true)) 
                return false;
            
            var tile = entity.GameArea.Tiles[mouseTileX, mouseTileY];
            
            if (!CloseEnoughToTile(entity, gameWorld, tile)) 
                return false;

            entity.GameArea.PlaceTile(tile, selectedItem.TileType);
            
            return true;
        }

        private static bool CanPlaceAt(IEntity entity, IGameWorld gameWorld, int tileX, int tileY,
            bool requiresImmediateNeighbour = false)
        {
            var tile = entity.GameArea.TryGetTile(tileX, tileY);

            if (tile == null) return false;

            if (requiresImmediateNeighbour && !tile.HasImmediateNeighbour())
                return false;

            return tile.TileType == TileTypes.None &&
                   !gameWorld.AnythingCollidesWith(tile);
        }

        private static bool CloseEnoughToTile(IEntity entity, IGameWorld gameWorld, Tile tile)
        {
            var playerBounds = entity.GetCurrentBounds();
            var playerLeft = playerBounds.Left;
            var playerRight = playerBounds.Right;
            var playerTop = playerBounds.Top;
            var playerBottom = playerBounds.Bottom;

            if (Math.Abs(tile.RenderCenter.X - playerLeft) > MaxDistanceToPlace * gameWorld.TileSize) return false;
            if (Math.Abs(tile.RenderCenter.X - playerRight) > MaxDistanceToPlace * gameWorld.TileSize) return false;
            if (Math.Abs(tile.RenderCenter.Y - playerTop) > MaxDistanceToPlace * gameWorld.TileSize) return false;
            return (Math.Abs(tile.RenderCenter.Y - playerBottom) <= MaxDistanceToPlace * gameWorld.TileSize);
        }
    }
}