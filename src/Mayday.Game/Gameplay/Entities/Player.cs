using Steamworks;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player : Entity
    {
        public int HeadId { get; set; } = 1;
        public int BodyId { get; set; } = 1;
        public int LegsId { get; set; } = 1;

        public override RectangleF GetBounds() =>
            new RectangleF(X + 18 * Game1.GlobalGameScale, Y + 18 * Game1.GlobalGameScale,
                42 * Game1.GlobalGameScale - 17 * Game1.GlobalGameScale - 18 * Game1.GlobalGameScale,
                33 * Game1.GlobalGameScale - 19 * Game1.GlobalGameScale);

        public void SetClientId()
        {
            EntityId = SteamClient.SteamId;
        }
    }
}