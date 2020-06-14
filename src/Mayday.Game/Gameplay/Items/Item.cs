using Mayday.Game.Enums;

namespace Mayday.Game.Gameplay.Items
{
    public class Item : IItem
    {
        public ItemType ItemId { get; set; }
        public string Name { get; set; }
        public int MaxStackSize { get; set; }
        public TileType TileType { get; set; } = TileType.None;

        public bool IsTheSameAs(IItem item)
            => item.ItemId == ItemId;
    }
}