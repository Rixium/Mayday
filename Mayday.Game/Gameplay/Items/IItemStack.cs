namespace Mayday.Game.Gameplay.Items
{
    public interface IItemStack
    {
        IItem Item { get; set; }
        int MaxStackSize { get; set; }
        int Count { get; set; }
        bool IsEmpty();
        bool ContainsItemOfType(IItem item);
        bool HasSpaceFor(IItem item);
        void AddItem(IItem item);
    }
}