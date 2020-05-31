using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.World
{
    public interface IGameWorld
    {
        int TileSize { get; set; }
        Tile[,] Tiles { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        void Move(Player player,
            int xMove,
            int yMove);
        Tile GetTileAt(int tileX, int tileY);
    }
}