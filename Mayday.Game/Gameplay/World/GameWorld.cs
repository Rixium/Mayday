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
    }
}