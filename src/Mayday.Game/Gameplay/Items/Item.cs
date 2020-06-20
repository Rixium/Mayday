using Mayday.Game.Enums;

namespace Mayday.Game.Gameplay.Items
{
    public class Item : IItem
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public int MaxStackSize { get; set; }
        public string TileType { get; set; } = TileTypes.None;
        public string WorldObjectType { get; set; }

        public bool IsTheSameAs(IItem item)
            => item.ItemId == ItemId;
    }
}