namespace Mayday.Game.Gameplay.Items
{
    public class Item : IItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxStackSize { get; set; }
        public int TileId { get; set; } = -1;

        public bool IsTheSameAs(IItem item)
            => item.Id == Id;
    }
}