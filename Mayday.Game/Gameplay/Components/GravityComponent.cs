using System.Diagnostics;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class GravityComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public float Gravity { get; set; } = 3.72f;

        public void Update()
        {
            var moveComponent = Player.GetComponent<MoveComponent>();
            var jumpComponent = Player.GetComponent<JumpComponent>();

            var activeGravity = Gravity;
            
            if (!Keyboard.GetState().IsKeyDown(Keys.Space) && jumpComponent.Jumping)
                 activeGravity = Gravity * 2;

            moveComponent.YVelocity -= activeGravity * Time.DeltaTime;
            
            moveComponent.YVelocity = MathHelper.Clamp(moveComponent.YVelocity, -activeGravity, moveComponent.YVelocity);
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}