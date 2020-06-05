namespace Mayday.Game.Gameplay.Items
{
    public interface IItem
    {
        int Id { get; set; }
        string Name { get; set; }
        int MaxStackSize { get; set; }
        bool IsTheSameAs(IItem item);
    }
}