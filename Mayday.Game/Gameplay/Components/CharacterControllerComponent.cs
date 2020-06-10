using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;

namespace Mayday.Game.Gameplay.Components
{
    public class CharacterControllerComponent : IComponent
    {
        private MoveComponent _moveComponent;
        private JumpComponent _jumpComponent;
        private KeyboardState _lastState;
        public IEntity Entity { get; set; }
        public void OnAddedToEntity()
        {
            _moveComponent = Entity.GetComponent<MoveComponent>();
            _jumpComponent = Entity.GetComponent<JumpComponent>();
            
            YetiGame.InputManager.RegisterInputEvent("MoveLeft", () => _moveComponent.SetMoveDirection(-1, 0), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent("MoveRight", () => _moveComponent.SetMoveDirection(1, 0), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent("MoveLeft", () => _moveComponent.SetMoveDirection(0, 0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent("MoveRight", () => _moveComponent.SetMoveDirection(0, 0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent("Jump", () => _jumpComponent.BeginJump());
        }
        
    }
}