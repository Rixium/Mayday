using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Graphics;

namespace Mayday.Game.Gameplay.Components
{
    public class PlayerAnimationComponent : IUpdateable
    {
        private MoveComponent _moveComponent;
        private JumpComponent _jumpComponent;
        public IAnimator HeadAnimator { get; set; }
        public IAnimator BodyAnimator { get; set; }
        public IAnimator LegsAnimator { get; set; }
        public IEntity Entity { get; set; }

        public void Update()
        {
            var speed = 0.1f;

            HeadAnimator?.Update(speed);
            BodyAnimator?.Update(speed);
            LegsAnimator?.Update(speed);

            if (_jumpComponent.Jumping)
            {
                HeadAnimator?.SetAnimation("Jump");
                BodyAnimator?.SetAnimation("Jump");
                LegsAnimator?.SetAnimation("Jump");
            }
            else if (Math.Abs(_moveComponent.XVelocity) > 0.01f)
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

        public void OnAddedToEntity()
        {
            _moveComponent = Entity.GetComponent<MoveComponent>();
            _jumpComponent = Entity.GetComponent<JumpComponent>();
        }
        
    }
}