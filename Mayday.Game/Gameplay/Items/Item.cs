namespace Mayday.Game.Gameplay.Items
{
    public class Item : IItem
    {
        public string Name { get; set; }
        public int MaxStackSize { get; set; }

        public bool IsTheSameAs(IItem item)
            => item.Name.Equals(Name);
    }
}