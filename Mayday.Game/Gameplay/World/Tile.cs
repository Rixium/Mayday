namespace Mayday.Game.Gameplay.World
{
    
    public enum TileType
    {
        NONE,
        GROUND,
        COPPER
    }
    
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType TileType { get; set; }
        
        public Tile(TileType tileType, int x, int y)
        {
            TileType = tileType;
            X = x;
            Y = y;
        }
    }
}