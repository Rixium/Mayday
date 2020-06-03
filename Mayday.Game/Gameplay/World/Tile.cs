using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.World
{
    public class Tile
    {
        public IGameWorld GameWorld { get; set; }
        public int TileSize => GameWorld.TileSize;
        public int RenderX => X * TileSize;
        public int RenderY => Y * TileSize;

        public int X { get; set; }
        public int Y { get; set; }

        private int _tileType;

        public int TileType
        {
            get => _tileType;
            set
            {
                if (_tileType == value)
                    return;

                _tileType = value;
                BlobValue = -1;

                SetNeighboursBlobFlag();
            }
        }

        private void SetNeighboursBlobFlag()
        {
            for (var i = X - 1; i <= X + 1; i++)
            {
                for (var j = Y - 1; j <= Y + 1; j++)
                {
                    if (i == X && j == Y) continue;
                    var tile = GameWorld.TryGetTile(i, j);
                    if (tile == null) continue;
                    tile.BlobValue = -1;
                }
            }
        }

        public int WallType { get; set; }

        public Vector2 RenderCenter => new Vector2(RenderX + TileSize / 2.0f, RenderY + TileSize / 2.0f);
        public int BlobValue { get; set; } = -1;

        public Tile(int tileType, int x, int y)
        {
            TileType = tileType;
            X = x;
            Y = y;
        }
        
        public RectangleF GetBounds() => new RectangleF(RenderX, RenderY, TileSize, TileSize);

    }
}