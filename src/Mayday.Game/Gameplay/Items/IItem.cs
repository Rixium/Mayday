using Mayday.Game.Enums;

namespace Mayday.Game.Gameplay.Items
{
    public interface IItem
    {
        ItemType Id { get; set; }
        string Name { get; set; }
        int MaxStackSize { get; set; }
        TileType TileType { get; set; }
        bool IsTheSameAs(IItem item);
    }
}