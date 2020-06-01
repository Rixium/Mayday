using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class MoveComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public float YVelocity { get; set; }
        public float XVelocity { get; private set; }

        public void Update()
        {
            var gameWorld = Player.GameWorld;
            
            var tileBelow = gameWorld.GetTileAt(Player.GetBounds().X / gameWorld.TileSize,
                (Player.GetBounds().Bottom) / gameWorld.TileSize + 1);

            if (Player.XDirection != 0 && tileBelow != null && tileBelow.TileType != TileType.NONE)
                XVelocity += Player.XDirection * 5 * Time.DeltaTime;
            else if (tileBelow == null || tileBelow.TileType != TileType.NONE)
                XVelocity = MathHelper.Lerp(0, XVelocity, 0.5f);
            else
                XVelocity = MathHelper.Lerp(0, XVelocity, 0.99999f);

            XVelocity = MathHelper.Clamp(XVelocity, -2, 2);

            var xMove = (int) XVelocity;

            var yMove = (int) -YVelocity;

            gameWorld.Move(Player, xMove, yMove);
        }
    }
}