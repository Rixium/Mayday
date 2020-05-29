using Yetiface.Engine.ECS.Components.Renderables;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player
    {
        public ulong SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int HeadId { get; set; } = 1;
        public int BodyId { get; set; } = 1;
        public int LegsId { get; set; } = 1;
        public int ArmsId { get; set; } = 1;
        
        public string Animation { get; set; }
        
    }
}