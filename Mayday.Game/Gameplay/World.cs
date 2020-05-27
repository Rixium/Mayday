namespace Mayday.Game.Gameplay
{
    public class World : IWorld
    {
        
        public Tile[,] Tiles { get; set; }
        
        /// <summary>
        /// The width and height are both in tiles, not pixels.
        /// </summary>
        public int Width { get; set; }
        public int Height { get; set; }
    }
}