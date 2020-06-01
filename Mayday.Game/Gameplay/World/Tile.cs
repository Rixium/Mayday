using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.World
{
    public enum TileType
    {
        NONE,
        GROUND,
        STONE
    }
    
    public enum WallType
    {
        NONE,
        GROUND
    }

    public class Tile
    {
        public IGameWorld GameWorld { get; set; }
        public int TileSize => GameWorld.TileSize;
        public int RenderX => X * TileSize;
        public int RenderY => Y * TileSize;

        public int X { get; set; }
        public int Y { get; set; }
        public TileType TileType { get; set; }
        public WallType WallType { get; set; }

        public Tile(TileType tileType, int x, int y)
        {
            TileType = tileType;
            X = x;
            Y = y;
        }

        public bool ContainsX(int boundsX,int width) => RenderX >= boundsX && RenderX + TileSize >= boundsX + width;

        public bool ContainsY(int boundsY, int height) => RenderY >= boundsY && RenderY + TileSize >= boundsY + height;

        public Rectangle GetBounds() => new Rectangle(RenderX, RenderY, TileSize, TileSize);
        
    }
}