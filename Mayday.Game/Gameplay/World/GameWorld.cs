using System;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
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

        public void Move(IPlayer player, float xMove, float yMove)
        {
            player.X += (int) xMove;

            var bounds = player.GetBounds();

            var tileStartX = (bounds.Left / TileSize) - 1;
            var tileEndX = (bounds.Right / TileSize) + 1;
            var tileStartY = (bounds.Top / TileSize) - 1;
            var tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = tileStartX; i <= tileEndX; i++)
            {
                for (var j = tileStartY; j <= tileEndY; j++)
                {
                    var tile = GetTileAt(i, j);
                    if (tile == null) continue;
                    if (tile.TileType == TileType.NONE) continue;

                    var tileBounds = tile.GetBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var canMoveUp = true;
                    var moveComponent = player.GetComponent<MoveComponent>();
                    
                    if (j >= tileEndY - 2 && Math.Abs(moveComponent.YVelocity) < 0.01f)
                    {
                        for (var k = j - 1; k > j - 4; k--)
                        {
                            var above = GetTileAt(i, k);

                            if (above.TileType == TileType.NONE)
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
                        player.X += (int) depth.X;
                        bounds = player.GetBounds();
                    }
                }
            }

            player.Y += (int) yMove;
            bounds = player.GetBounds();

            tileStartX = (bounds.Left / TileSize) - 1;
            tileEndX = (bounds.Right / TileSize) + 1;
            tileStartY = (bounds.Top / TileSize) - 1;
            tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = tileStartX; i <= tileEndX; i++)
            {
                for (var j = tileStartY; j <= tileEndY; j++)
                {
                    var tile = GetTileAt(i, j);
                    if (tile == null) continue;
                    if (tile.TileType == TileType.NONE) continue;

                    var tileBounds = tile.GetBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var depth = bounds.GetIntersectionDepth(tileBounds);
                    player.Y += (int) depth.Y;
                    bounds = player.GetBounds();
                }
            }
        }

        public Tile GetTileAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return Tiles[x, y];
        }
    }
}