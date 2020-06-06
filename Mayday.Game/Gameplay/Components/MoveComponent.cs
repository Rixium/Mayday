using System;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Components
{
    public class MoveComponent : IComponent
    {
        
        public IEntity Entity { get; set; }
        public float YVelocity { get; set; }
        public float XVelocity { get; set; }
        public Action HitFloor { get; set; }

        public void Update()
        {
            var gameWorld = Entity.GameWorld;

            if (Entity.XDirection != 0)
                XVelocity += 0.1f * Entity.XDirection * Game1.GlobalGameScale;
            else
            {
                XVelocity *= 0.9f;

                if (Math.Abs(XVelocity) < 1f)
                    XVelocity = 0;
            }

            XVelocity = MathHelper.Clamp(XVelocity, -1f * Game1.GlobalGameScale, 1f * Game1.GlobalGameScale);

            CheckPlayerHit();
            
            
            var xMove = XVelocity;
            var yMove = -YVelocity;

            gameWorld.Move(Entity, xMove, yMove);
        }


        private void CheckPlayerHit()
        {
            var playerBounds = Entity.GetBounds();

            var tileStartX = (playerBounds.Left + 1) / Entity.GameWorld.TileSize;
            var tileEndX = (playerBounds.Right - 1) / Entity.GameWorld.TileSize;
            
            if(YVelocity > 0) // Travelling Upwards
                for (var i = (int) tileStartX; i <= tileEndX; i++)
                {
                    var tile = Entity.GameWorld.TryGetTile(i,
                        (int) ((playerBounds.Top - YVelocity) / Entity.GameWorld.TileSize));
                    if (tile != null && tile.TileType == 0) 
                        continue;
                    
                    YVelocity = 0;
                    return;
                }
            else if (YVelocity < 0) // Travelling Downwards
                for (var i = (int) tileStartX; i <= tileEndX; i++)
                {
                    var tile = Entity.GameWorld.TryGetTile(i,
                        (int) ((playerBounds.Bottom  + 1) / Entity.GameWorld.TileSize));
                    if (tile != null && tile.TileType == 0) continue;
                    
                    HitFloor?.Invoke();
                    return;
                }
        }
        
        public void OnAddedToPlayer()
        {
            
        }
    }
}