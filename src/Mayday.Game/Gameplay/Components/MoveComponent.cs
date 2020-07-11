using System;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Components
{
    public class MoveComponent : IUpdateable
    {
        
        public IEntity Entity { get; set; }
        public float YVelocity { get; set; }
        public float XVelocity { get; set; }
        public Action HitFloor { get; set; }
        public Action<MoveComponent> MoveDirectionChanged { get; set; }
        public Action<MoveComponent> PositionChanged { get; set; }
        public bool Grounded { get; set; }

        private float _maxVelocity = 1.5f;
        private float _velocityChange = 0.2f;

        public void Update()
        {
            XVelocity *= 0.9f;

            if (Entity.XDirection != 0)
            {
                    XVelocity += _velocityChange * Entity.XDirection;
            }
            else
            {
                XVelocity *= 0.9f;

                if (Math.Abs(XVelocity) < 1f)
                    XVelocity = 0;
            }

            XVelocity = MathHelper.Clamp(XVelocity, -_maxVelocity, _maxVelocity);

            CheckPlayerHit();

            var currentY = Entity.Y;
            var currentX = Entity.X;
            
            var xMove = XVelocity;
            var yMove = -YVelocity;
            
            Entity.GameArea.Move(Entity, xMove, yMove, YVelocity);
            Grounded = Math.Abs(currentY - Entity.Y) < 0.01f;

            if (Grounded)
                YVelocity = 0;
            
            if (Math.Abs(currentX - Entity.X) > 0.01f || Math.Abs(currentY - Entity.Y) > 0.01f)
            {
                PositionChanged?.Invoke(this);
            }
        }


        private void CheckPlayerHit()
        {
            var playerBounds = Entity.GetCurrentBounds();

            var tileStartX = (playerBounds.Left + 1) / Entity.GameWorld.TileSize;
            var tileEndX = (playerBounds.Right - 1) / Entity.GameWorld.TileSize;
            
            if(YVelocity > 0) // Travelling Upwards
                for (var i = (int) tileStartX; i <= tileEndX; i++)
                {
                    var tile = Entity.GameArea.TryGetTile(i,
                        (int) ((playerBounds.Top - YVelocity) / Entity.GameWorld.TileSize));
                    if (tile != null && tile.TileType == TileTypes.None)
                        continue;
                    
                    YVelocity = 0;
                    return;
                }
            else if (YVelocity < 0) // Travelling Downwards
                for (var i = (int) tileStartX; i <= tileEndX; i++)
                {
                    var tile = Entity.GameArea.TryGetTile(i,
                        (int) ((playerBounds.Bottom  + 1) / Entity.GameWorld.TileSize));
                    if (tile != null && tile.TileType == TileTypes.None) continue;
                    
                    HitFloor?.Invoke();
                    return;
                }
        }
        
        public void OnAddedToEntity()
        {
            
        }

        public void SetMoveDirection(int x, int y)
        {
            if (x != 0)
            {
                Entity.FacingDirection = x;
            }
            
            if (Entity.XDirection == x) return;
            Entity.XDirection = x;
            MoveDirectionChanged?.Invoke(this);
        }
    }
}