using Mayday.Game.Enums;

namespace Mayday.Game.Gameplay.Data
{
    public class TileProperties
    {
        public string Name { get; set; }
        public bool CanMine { get; set; }
        public float MineSpeedModifier { get; set; }
        public ItemType ItemDropType { get; set; } = ItemType.None;
    }
}