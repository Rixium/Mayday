namespace Mayday.Game.Gameplay.Items
{
    public class Item : IItem
    {
        public string ItemId { get; set; } = "";
        public string Name { get; set; } = "";
        public int MaxStackSize { get; set; } = 1;
        public string TileType { get; set; }
        public string WorldObjectType { get; set; }

        public bool IsTheSameAs(IItem item)
            => item.ItemId == ItemId;
    }
}