namespace Mayday.Game.Gameplay.Data
{
    public class TileProperties
    {
        public string Name { get; set; }
        public bool Mineable { get; set; }
        public float MineSpeedModifier { get; set; }
        public int ItemDropId { get; set; } = -1;
    }
}