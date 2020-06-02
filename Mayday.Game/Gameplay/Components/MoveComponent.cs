using System;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Components
{
    public class MoveComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public float YVelocity { get; set; }
        public float XVelocity { get; private set; }
        public Action HitFloor { get; set; }

        public void Update()
        {
            var gameWorld = Player.GameWorld;

            if (Player.XDirection != 0)
                XVelocity += 0.1f * Player.XDirection;
            else
            {
                XVelocity *= 0.9f;

                if (Math.Abs(XVelocity) < 1f)
                    XVelocity = 0;
            }

            XVelocity = MathHelper.Clamp(XVelocity, -1f, 1f);
            
            if (PlayerHit(Player, YVelocity))
                YVelocity = 0;
            
            var xMove = XVelocity;
            var yMove = -YVelocity;

            gameWorld.Move(Player, xMove, yMove);
        }


        private bool PlayerHit(IPlayer player, float yVelocity)
        {
            var playerBounds = Player.GetBounds();

            var tileStartX = (playerBounds.Left + 1) / player.GameWorld.TileSize;
            var tileEndX = (playerBounds.Right - 1) / player.GameWorld.TileSize;
            
            if(yVelocity > 0) // Travelling Upwards
                for (var i = (int) tileStartX; i <= tileEndX; i++)
                {
                    var tile = Player.GameWorld.TryGetTile(i,
                        (int) ((playerBounds.Top - yVelocity) / player.GameWorld.TileSize));
                    if (tile == null || tile.TileType != 0) return true;
                }
            else if (yVelocity < 0) // Travelling Downwards
                for (var i = (int) tileStartX; i <= tileEndX; i++)
                {
                    var tile = Player.GameWorld.TryGetTile(i,
                        (int) ((playerBounds.Bottom  + 1) / player.GameWorld.TileSize));
                    if (tile != null && tile.TileType == 0) continue;
                    HitFloor?.Invoke();
                    return true;
                }

            // Didn't hit, so we good!
            return false;
        }
        
        public void OnAddedToPlayer()
        {
            
        }
    }
}