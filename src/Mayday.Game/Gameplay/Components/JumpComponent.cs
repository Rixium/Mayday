using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework.Audio;
using Yetiface.Engine;

namespace Mayday.Game.Gameplay.Components
{
    public class JumpComponent : IComponent
    {
        
        private MoveComponent _moveComponent;

        public Action<JumpComponent> Jump;

        public IEntity Entity { get; set; }
        public bool Jumping { get; set; }

        public void OnAddedToEntity()
        {
            // We need to know when the player hits the floor, so that we can
            // set jumping back to false.
            _moveComponent = Entity.GetComponent<MoveComponent>();
            _moveComponent.HitFloor += () => Jumping = false;
        }

        public void BeginJump()
        {
            if (Jumping) return;
            
            var moveComponent = Entity.GetComponent<MoveComponent>();

            if (!moveComponent.Grounded) return;
            
            Jumping = true;
            moveComponent.YVelocity = 2 * Game1.GlobalGameScale;

            YetiGame.ContentManager.Load<SoundEffect>("jump").Play();
            
            Jump?.Invoke(this);
        }

        public void EndJump()
        {
            _moveComponent.YVelocity *= 0.5f;
        }
        
    }
}