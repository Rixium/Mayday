using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class GravityComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public float Gravity { get; set; } = 9.8f;

        public float Gravity2 => Gravity * 2;
        public Action HitFloor { get; set; }

        public void Update()
        {
            var moveComponent = Player.GetComponent<MoveComponent>();

            var activeGravity = Gravity;
            
            if (!Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                activeGravity = Gravity2;
            }
            
            moveComponent.YVelocity -= activeGravity * Time.DeltaTime;
            
            moveComponent.YVelocity = MathHelper.Clamp(moveComponent.YVelocity, -activeGravity, moveComponent.YVelocity);

            if (PlayerHit(Player, moveComponent.YVelocity))
                moveComponent.YVelocity = 0;

        }

        public void OnAddedToPlayer()
        {
            
        }

        private bool PlayerHit(IPlayer player, float yVelocity)
        {
            var playerBounds = Player.GetBounds();

            var tileStartX = (playerBounds.Left + 1) / player.GameWorld.TileSize;
            var tileEndX = (playerBounds.Right - 1) / player.GameWorld.TileSize;
            
            if(yVelocity > 0) // Travelling Upwards
                for (var i = tileStartX; i <= tileEndX; i++)
                {
                    var tile = Player.GameWorld.GetTileAt(i,
                        (int) ((playerBounds.Top - yVelocity) / player.GameWorld.TileSize));
                    if (tile == null || tile.TileType != TileType.NONE) return true;
                }
            else if (yVelocity < 0) // Travelling Downwards
                for (var i = tileStartX; i <= tileEndX; i++)
                {
                    var tile = Player.GameWorld.GetTileAt(i,
                        (int) ((playerBounds.Bottom - yVelocity) / player.GameWorld.TileSize));
                    if (tile != null && tile.TileType == TileType.NONE) continue;
                    HitFloor?.Invoke();
                    return true;
                }

            return false;
        }
    }
}