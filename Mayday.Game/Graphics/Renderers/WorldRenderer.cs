using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class WorldRenderer : IWorldRenderer
    {
        
        private readonly Dictionary<int, int> _tileBlobMap = new Dictionary<int, int>
        {
            {28, 0}, {124, 1}, {112, 2}, {16, 3}, {20, 4}, {116, 5}, {92, 6}, {80, 7}, {84, 8}, {221, 9},
            {31, 11}, {255, 12}, {241, 13}, {17, 14}, {23, 15}, {247, 16}, {223, 17}, {209, 18}, {215, 19}, {119, 20},
            {7, 22}, {199, 23}, {193, 24}, {1, 25}, {29, 26}, {253, 27}, {127, 28}, {113, 29}, {125, 30}, {93, 31},
            {117, 32},
            {4, 33}, {68, 34}, {64, 35}, {0, 36}, {5, 37}, {197, 38}, {71, 39}, {65, 40}, {69, 41}, {87, 42}, {213, 43},
            {21, 48}, {245, 49}, {95, 50}, {81, 51}, {85, 52}
        };

        public void Draw(IGameWorld gameWorld, Camera camera)
        {
            var worldTileSize = gameWorld.TileSize;

            var startTileX = (int) (camera.Position.X - Window.ViewportWidth / 2.0f) / worldTileSize - 1;
            var startTileY = (int) (camera.Position.Y - Window.ViewportHeight / 2.0f) / worldTileSize - 1;
            var endTileX = (int) (camera.Position.X + Window.ViewportWidth / 2.0f) / worldTileSize - 1;
            var endTileY = (int) (camera.Position.Y + Window.ViewportHeight / 2.0f) / worldTileSize - 1;

            for (var i = startTileX; i < endTileX; i++)
            {
                for (var j = startTileY; j < endTileY; j++)
                {
                    if (i < 0 || j < 0 || i > gameWorld.Width - 1 || j > gameWorld.Height - 1) continue;
                    var tile = gameWorld.Tiles[i, j];

                    if (tile.TileType == TileType.NONE) continue;

                    var tileIndex = GetTileBlobValue(gameWorld, tile, _tileBlobMap);

                    var tileSet = ContentChest.Tiles[(int) tile.TileType];

                    var sheetTileSize = 16;
                    IndexConvert(tileIndex, tileSet.Width / sheetTileSize, out var x, out var y);
                    var rect = new Rectangle(x * sheetTileSize, y * sheetTileSize, sheetTileSize, sheetTileSize);

                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet,
                        new Rectangle(tile.RenderX, tile.RenderY, tile.TileSize, tile.TileSize),
                        rect, Color.White);
                }
            }
        }
        
        
        // Gets the blob value from a given tile blob map for a given tile.
        private static int GetTileBlobValue(IGameWorld gameWorld, Tile tile, Dictionary<int, int> tileBlobMap)
        {
            var x = tile.X;
            var y = tile.Y;
            byte bitSum = 0;

            var n = TryGetTile(gameWorld, x, y - 1);
            var e = TryGetTile(gameWorld, x + 1, y);
            var s = TryGetTile(gameWorld, x, y + 1);
            var w = TryGetTile(gameWorld, x - 1, y);
            var nw = TryGetTile(gameWorld, x - 1, y - 1);
            var ne = TryGetTile(gameWorld, x + 1, y - 1);
            var se = TryGetTile(gameWorld, x + 1, y + 1);
            var sw = TryGetTile(gameWorld, x - 1, y + 1);

            TileTypeMatch(ref bitSum, tile, n, 1);
            TileTypeMatch(ref bitSum, tile, e, 4);
            TileTypeMatch(ref bitSum, tile, s, 16);
            TileTypeMatch(ref bitSum, tile, w, 64);
            TileTypeMatch(ref bitSum, tile, se, 8, e, s);
            TileTypeMatch(ref bitSum, tile, ne, 2, n, e);
            TileTypeMatch(ref bitSum, tile, sw, 32, s, w);
            TileTypeMatch(ref bitSum, tile, nw, 128, n, w);

            tileBlobMap.TryGetValue(bitSum, out var tileNumber);
            
            return tileNumber;
        }

        // Attempt to get a given tile index x, y from a given game world.
        // Returns null if it doesn't exist. (Out of bounds).
        private static Tile TryGetTile(IGameWorld gameWorld, int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0) return null;
            if (tileX > gameWorld.Width - 1 || tileY > gameWorld.Height - 1) return null;
            return gameWorld.Tiles[tileX, tileY];
        }

        // Convert a 1D array index to a 2D array index.
        private static void IndexConvert(int index, int arrayWidth, out int x, out int y)
        {
            x = index % arrayWidth;
            y = index / arrayWidth;
        }

        // Checks if a tile matches another tile, or any of the assure not tiles passed.
        // If they do, then add the bit value to the bit sum. Used for Finding blob tile.
        private static void TileTypeMatch(ref byte bitSum, Tile tile1, Tile tile2, byte bitValue, params Tile[] assureNot)
        {
            if (tile2 == null) return;
            if (tile1 == null) return;
            if (!tile1.TileType.Equals(tile2.TileType)) return;
            if (assureNot.Any(tile => tile.TileType != tile1.TileType)) return;

            bitSum += bitValue;
        }
        
    }
}