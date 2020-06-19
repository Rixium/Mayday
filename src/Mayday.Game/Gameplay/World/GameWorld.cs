using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.World
{
    public class GameWorld : IGameWorld
    {
        public int TileSize { get; set; }
        public Tile[,] Tiles { get; set; }

        /// <summary>
        /// The width and height are both in tiles, not pixels.
        /// </summary>
        public int Width { get; set; }

        public int Height { get; set; }
        public IWorldItemSet WorldItems { get; set; } = new WorldItemSet();
        public Action<Tile> TilePlaced { get; set; }
        public HashSet<IEntity> WorldEntities { get; } = new HashSet<IEntity>();

        public void Move(IEntity player, float xMove, float yMove, float yVelocity)
        {
            player.X += xMove;

            var bounds = player.GetBounds();

            var tileStartX = (bounds.Left / TileSize) - 1;
            var tileEndX = (bounds.Right / TileSize) + 1;
            var tileStartY = (bounds.Top / TileSize) - 1;
            var tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = (int) tileStartX; i <= tileEndX; i++)
            {
                for (var j = (int) tileStartY; j <= tileEndY; j++)
                {
                    var tile = TryGetTile(i, j);
                    if (tile == null) continue;
                    if (tile.TileType == TileType.None) continue;

                    var tileBounds = tile.GetBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var canMoveUp = true;
                    if (j >= tileEndY - 2 && Math.Abs(yVelocity) < 0.01f)
                    {
                        for (var k = j - 1; k > j - 4; k--)
                        {
                            var above = TryGetTile(i, k);

                            if (above.TileType == TileType.None)
                                continue;

                            canMoveUp = false;
                            break;
                        }
                    }
                    else
                    {
                        canMoveUp = false;
                    }

                    if (canMoveUp)
                    {
                        yMove--;
                    }
                    else
                    {
                        var depth = bounds.GetIntersectionDepth(tileBounds);
                        player.X += depth.X;
                        bounds = player.GetBounds(); 
                    }
                }
            }

            player.Y += yMove;
            bounds = player.GetBounds();

            tileStartX = (bounds.Left / TileSize) - 1;
            tileEndX = (bounds.Right / TileSize) + 1;
            tileStartY = (bounds.Top / TileSize) - 1;
            tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = (int) tileStartX; i <= tileEndX; i++)
            {
                for (var j = (int) tileStartY; j <= tileEndY; j++)
                {
                    var tile = TryGetTile(i, j);
                    if (tile == null) continue;
                    if (tile.TileType == TileType.None) continue;

                    var tileBounds = tile.GetBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var depth = bounds.GetIntersectionDepth(tileBounds);
                    player.Y += depth.Y;
                    bounds = player.GetBounds();
                }
            }
        }

        public Tile TryGetTile(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0) return null;
            if (tileX > Width - 1 || tileY > Height - 1) return null;
            return Tiles[tileX, tileY];
        }
        
        public void PlaceTile(Tile tile, TileType tileType)
        {
            if (tile.TileType == tileType) return;
            tile.TileType = tileType;
            TilePlaced?.Invoke(tile);
        }

        public Tile GetRandomSpawnLocation() => (from Tile tile in Tiles
                    where tile.TileType == TileType.Dirt
                    select Tiles[(int) (Width / 2.0f), tile.TileY])
                .FirstOrDefault();

        public void DropItem(ItemDrop item)
        {
            item.GameWorld = this;
            WorldItems.Add(item);
        }

        public bool AnythingCollidesWith(Tile tile) =>
            WorldEntities.Any(entity => tile.GetBounds().Intersects(entity.GetBounds()));

        public void AddTrackedEntity(Player player) => WorldEntities.Add(player);

    }
}