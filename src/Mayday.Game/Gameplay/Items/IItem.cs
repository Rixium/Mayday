using Mayday.Game.Enums;

namespace Mayday.Game.Gameplay.Items
{
    public interface IItem
    {
        ItemType ItemId { get; set; }
        string Name { get; set; }
        int MaxStackSize { get; set; }
        TileType TileType { get; set; }
        WorldObjectType WorldObjectType { get; set; }
        bool IsTheSameAs(IItem item);
    }
}