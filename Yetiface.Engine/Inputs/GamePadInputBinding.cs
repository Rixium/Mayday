using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Yetiface.Engine.Inputs
{
    public class GamePadInputBinding : IInputBinding
    {
        private GamePadState _lastState;
        
        private readonly int _gamePadIndex;
        private readonly Buttons _gamePadButton;
        
        public string Name { get; set; }
        public IList<Action> OnPressed { get; set; }
        public IList<Action> OnReleased { get; set; }
        public IList<Action> OnHeld { get; set; }

        public GamePadInputBinding(int gamePadIndex, Buttons gamePadButton)
        {
            OnPressed = new List<Action>();
            OnReleased = new List<Action>();
            OnHeld = new List<Action>();
            
            _gamePadIndex = gamePadIndex;
            _gamePadButton = gamePadButton;
        }
        
        public void Update()
        {
            var state = GamePad.GetState(_gamePadIndex);

            if(state.IsButtonDown(_gamePadButton) && _lastState.IsButtonUp(_gamePadButton))
                foreach(var action in OnPressed)
                    action?.Invoke();
            else if(state.IsButtonUp(_gamePadButton) && _lastState.IsButtonDown(_gamePadButton))
                foreach(var action in OnReleased)
                    action?.Invoke();

            _lastState = state;
        }
    }
}