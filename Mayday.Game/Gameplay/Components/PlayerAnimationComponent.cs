using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Components
{
    public class PlayerAnimationComponent : IComponent
    {
        public IPlayer Player { get; set; }
        public IAnimator HeadAnimator { get; set; }
        public IAnimator BodyAnimator { get; set; }
        public IAnimator LegsAnimator { get; set; }

        public void Update()
        {
            var moveComponent = Player.GetComponent<MoveComponent>();
            var jumpComponent = Player.GetComponent<JumpComponent>();

            var speed = 0.1f;
            
            HeadAnimator?.Update(speed);
            BodyAnimator?.Update(speed);
            LegsAnimator?.Update(speed);
            
            if (jumpComponent.Jumping)
            {
                HeadAnimator?.SetAnimation("Jump");
                BodyAnimator?.SetAnimation("Jump");
                LegsAnimator?.SetAnimation("Jump");
            } else if (Math.Abs(moveComponent.XVelocity) > 0.01f)
            {
                HeadAnimator?.SetAnimation("Walk");
                BodyAnimator?.SetAnimation("Walk");
                LegsAnimator?.SetAnimation("Walk");
            }
            else
            {
                HeadAnimator?.SetAnimation("Idle");
                BodyAnimator?.SetAnimation("Idle");
                LegsAnimator?.SetAnimation("Idle");
            }
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}