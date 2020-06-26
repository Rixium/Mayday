using System.Collections.Generic;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.World.Areas
{
    public interface IGameArea
    {
        IGameWorld GameWorld { get; set; }
        IWorldItemSet WorldItems { get; set; }
        IWorldObjectSet WorldObjects { get; set; }
        Tile[,] Tiles { get; set; }
        int AreaHeight { get; set; }
        int AreaWidth { get; set; }
        void Move(IEntity entity, float xMove, float yMove, float yVelocity);
        Tile TryGetTile(int x, int y);
        IEntity GetWorldObjectAbove(Tile tile);
        void PlaceTile(Tile tile, string selectedItemTileType);
        void PlaceWorldEntity(Tile tile, string selectedItemWorldObjectType);
        void DropItem(ItemDrop itemDrop);
        Tile GetRandomSpawnLocation();
        IEnumerable<IEntity> GetItems();
    }
}