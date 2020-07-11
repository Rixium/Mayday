namespace Mayday.Game.Gameplay.Items
{
    public interface IItem
    {
        string ItemId { get; set; }
        string Name { get; set; }
        int MaxStackSize { get; set; }
        string TileType { get; set; }
        string WorldObjectType { get; set; }
        bool IsTheSameAs(IItem item);
    }
}