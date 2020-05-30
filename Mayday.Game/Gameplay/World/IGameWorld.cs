namespace Mayday.Game.Gameplay.World
{
    public interface IGameWorld
    {
        int TileSize { get; set; }
        Tile[,] Tiles { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}