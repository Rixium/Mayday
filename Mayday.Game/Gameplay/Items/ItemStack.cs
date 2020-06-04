namespace Mayday.Game.Gameplay.Items
{
    public class ItemStack : IItemStack
    {
        public int MaxStackSize { get; set; } = 1;
        public int Count { get; set; }
        public IItem Item { get; set; }

        public bool IsEmpty() => Item == null;

        public bool ContainsItemOfType(IItem item)
        {
            return !IsEmpty() && Item.IsTheSameAs(item);
        }

        public bool HasSpaceFor(IItem item)
        {
            if (!IsEmpty() && !ContainsItemOfType(item)) return false;
            return Count < MaxStackSize;
        }

        public void AddItem(IItem item)
        {
            if (!IsEmpty() && !ContainsItemOfType(item)) return;
            if (Count >= MaxStackSize) return;
            
            Item = item;
            Count++;
            MaxStackSize = item.MaxStackSize;
        }
    }
}