namespace Mayday.Game.Gameplay
{
    public interface IWorld
    {
        Tile[,] Tiles { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}