using Mayday.Game.Enums;

namespace Mayday.Game.Gameplay.Data
{
    public class WorldObjectData
    {
        public WorldObjectType WorldObjectType { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}