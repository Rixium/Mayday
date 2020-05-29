namespace Mayday.Game.Gameplay.Entities
{
    public class Player
    {
        
        public ulong SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public int HeadId { get; set; }
        public int TorsoId { get; set; }
        public int LegsId { get; set; }
        public int ArmsId { get; set; }
        
    }
}