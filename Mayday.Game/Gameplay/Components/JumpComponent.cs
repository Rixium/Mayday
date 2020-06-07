using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework.Input;

namespace Mayday.Game.Gameplay.Components
{
    public class JumpComponent : IComponent
    {
        private MoveComponent _moveComponent;
        private KeyboardState _lastKeyboardState;

        public IEntity Entity { get; set; }
        public bool Jumping { get; set; }

        public void Update()
        {
            if (Jumping)
            {
                if (_lastKeyboardState.IsKeyDown(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    if (_moveComponent.YVelocity > 0)
                        _moveComponent.YVelocity *= 0.5f;
                }
            }

            _lastKeyboardState = Keyboard.GetState();
        }

        public void OnAddedToPlayer()
        {
            // We need to know when the player hits the floor, so that we can
            // set jumping back to false.
            _moveComponent = Entity.GetComponent<MoveComponent>();
            _moveComponent.HitFloor += () => Jumping = false;
        }

        public void Jump()
        {
            if (Jumping) return;
            
            var moveComponent = Entity.GetComponent<MoveComponent>();
            
            Jumping = true;
            moveComponent.YVelocity = 2 * Game1.GlobalGameScale;
        }

    }
}