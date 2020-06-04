namespace Mayday.Game.Gameplay.Items
{
    public interface IItem
    {
        
        string Name { get; set; }
        int MaxStackSize { get; set; }
        bool IsTheSameAs(IItem item);
    }
}