using Mayday.Game.Gameplay.WorldMakers;

namespace Mayday.Game.Gameplay.World
{
    public interface IGameWorld
    {
        Tile[,] Tiles { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}