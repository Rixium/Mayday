namespace Mayday.Game.Gameplay.Data
{
    public class WorldObjectData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool CanBeUsed { get; set; }

        public BuildNode[] BuildNodes { get; set; }
    }
}