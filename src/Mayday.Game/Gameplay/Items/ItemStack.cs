namespace Mayday.Game.Gameplay.Items
{
    public class ItemStack : IItemStack
    {
        public int MaxStackSize { get; set; } = 1;
        public int Count { get; set; }
        public IItem Item { get; set; }

        public bool IsEmpty() => Item == null;

        public bool ContainsItemOfType(IItem item) =>
            Item != null && Item.IsTheSameAs(item);

        public bool HasSpaceFor(IItem item)
        {
            if (Count >= MaxStackSize) return false;
            return ContainsItemOfType(item) || IsEmpty();
        }

        public bool AddItem(IItem item)
        {
            if (Count >= MaxStackSize) return false;
            if (Item != null && !ContainsItemOfType(item)) return false;

            Item = item;
            Count++;
            MaxStackSize = item.MaxStackSize;
            
            return true;
        }
    }
}