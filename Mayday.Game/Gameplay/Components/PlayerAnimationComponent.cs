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

            var speed = (float) (1.0 / Math.Pow(5, Math.Abs(moveComponent.XVelocity)));
            speed = MathHelper.Clamp(speed, 0.05f, 0.2f);
            
            HeadAnimator?.Update(speed);
            BodyAnimator?.Update(speed);
            LegsAnimator?.Update(speed);

            if (Math.Abs(moveComponent.XVelocity) > 0.01f)
            {
                HeadAnimator?.SetAnimation("Walk");
                BodyAnimator?.SetAnimation("Walk");
                LegsAnimator?.SetAnimation("Walk");
            }
            else
            {
                HeadAnimator?.StopAnimation();
                BodyAnimator?.StopAnimation();
                LegsAnimator?.StopAnimation();
            }
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}