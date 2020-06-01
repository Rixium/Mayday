using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Components
{
    public class GravityComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public void Update()
        {
            var moveComponent = Player.GetComponent<MoveComponent>();

            moveComponent.YVelocity = MathHelper.Lerp(-11, moveComponent.YVelocity, 0.99f);

            var tileBelow = Player.GameWorld.GetTileAt(Player.GetBounds().X / Player.GameWorld.TileSize,
                (Player.GetBounds().Bottom - (int) moveComponent.YVelocity) / Player.GameWorld.TileSize);

            if (tileBelow == null ||
                tileBelow.TileType != TileType.NONE)
                moveComponent.YVelocity = 0;
        }
    }
}