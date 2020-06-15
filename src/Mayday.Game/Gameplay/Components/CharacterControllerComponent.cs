using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking;
using Mayday.Game.Networking.Packets;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;

namespace Mayday.Game.Gameplay.Components
{
    public class CharacterControllerComponent : IUpdateable
    {
        private MoveComponent _moveComponent;
        private JumpComponent _jumpComponent;
        private KeyboardState _lastKeyboardState;
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

        public void Update()
        {
            if(_jumpComponent.Jumping) {
                if (_lastKeyboardState.IsKeyDown(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    if (_moveComponent.YVelocity > 0)
                    {
                        _jumpComponent.EndJump();
                        
                        var jumpPacket = new JumpPacket
                        {
                            IsStopping = true
                        };

                        PacketManager.SendJumpStatePacket(jumpPacket);
                    }
                }
            }

            _lastKeyboardState = Keyboard.GetState();
        }
    }
}