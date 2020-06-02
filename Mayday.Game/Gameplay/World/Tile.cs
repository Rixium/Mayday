using Microsoft.Xna.Framework;

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
        public int TileType { get; set; }

        public int WallType { get; set; }

        public Vector2 RenderCenter => new Vector2(RenderX + TileSize / 2.0f, RenderY + TileSize / 2.0f);

        public Tile(int tileType, int x, int y)
        {
            TileType = tileType;
            X = x;
            Y = y;
        }
        
        public Rectangle GetBounds() => new Rectangle(RenderX, RenderY, TileSize, TileSize);

    }
}