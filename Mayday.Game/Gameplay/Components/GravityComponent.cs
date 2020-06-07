using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class GravityComponent : IUpdateable
    {
        public float Gravity { get; set; } = 3.72f * Game1.GlobalGameScale;

        public IEntity Entity { get; set; }

        public void Update()
        {
            var moveComponent = Entity.GetComponent<MoveComponent>();

            var activeGravity = Gravity;
            
            moveComponent.YVelocity -= activeGravity * Time.DeltaTime;
            
            moveComponent.YVelocity = MathHelper.Clamp(moveComponent.YVelocity, -activeGravity, moveComponent.YVelocity);
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}