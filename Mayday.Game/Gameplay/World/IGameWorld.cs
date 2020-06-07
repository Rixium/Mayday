using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.World
{
    public interface IGameWorld
    {
        Action<Tile> TilePlaced { get; set; }
        int TileSize { get; set; }
        Tile[,] Tiles { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        IList<IEntity> WorldItems { get; set; }
        void Move(IEntity player,
            float xMove,
            float yMove);
        Tile TryGetTile(int tileX, int tileY);
        void PlaceTile(Tile tile, int tileType);
    }
}