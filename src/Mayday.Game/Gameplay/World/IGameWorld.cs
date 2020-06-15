﻿using System;
using System.Collections.Generic;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.World
{
    public interface IGameWorld
    {
        Action<Tile> TilePlaced { get; set; }
        int TileSize { get; set; }
        Tile[,] Tiles { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        IWorldItemSet WorldItems { get; set; }
        void Move(IEntity player,
            float xMove,
            float yMove,
            float yVelocity);
        Tile TryGetTile(int tileX, int tileY);
        void PlaceTile(Tile tile, TileType tileType);
        Tile GetRandomSpawnLocation();
        void DropItem(ItemDrop item);
    }
}