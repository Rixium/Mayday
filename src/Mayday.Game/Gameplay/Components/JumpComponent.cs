using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework.Input;

namespace Mayday.Game.Gameplay.Components
{
    public class JumpComponent : IUpdateable
    {
        private readonly GameScreen _gameScreen;
        private MoveComponent _moveComponent;
        private KeyboardState _lastKeyboardState;

        public Action<JumpComponent> Jump;

        public IEntity Entity { get; set; }
        public bool Jumping { get; set; }

        public JumpComponent(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        public void Update()
        {
            if (Entity != _gameScreen.MyPlayer) return;
            
            if (Jumping)
            {
                if (_lastKeyboardState.IsKeyDown(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    if (_moveComponent.YVelocity > 0)
                    {
                        EndJump();
                        
                        var jumpPacket = new JumpPacket
                        {
                            IsStopping = true
                        };

                        _gameScreen.JumpStateChange(jumpPacket);
                    }
                }
            }

            _lastKeyboardState = Keyboard.GetState();
        }

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
            
            Jump?.Invoke(this);
        }

        public void EndJump()
        {
            _moveComponent.YVelocity *= 0.5f;
        }
        
    }
}