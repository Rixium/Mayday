using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Yetiface.Engine.Inputs
{
    public class KeyInputBinding : IInputBinding
    {

        private KeyboardState _lastKeyboardState;
        public Keys Key { get; set; }

        public KeyInputBinding(Keys key)
        {
            OnPressed = new List<Action>();
            OnReleased = new List<Action>();
            OnHeld = new List<Action>();
            
            Key = key;
        }

        public string Name { get; set; }
        public IList<Action> OnPressed { get; set; }
        public IList<Action> OnReleased { get; set; }
        public IList<Action> OnHeld { get; set; }
        
        public void Update()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key) && _lastKeyboardState.IsKeyUp(Key))
                foreach (var action in OnPressed)
                    action?.Invoke();
            else if (keyState.IsKeyUp(Key) && _lastKeyboardState.IsKeyDown(Key))
                foreach (var action in OnReleased)
                    action?.Invoke();
            else if (keyState.IsKeyDown(Key) && _lastKeyboardState.IsKeyDown(Key))
                foreach(var action in OnHeld) action?.Invoke();
            
            _lastKeyboardState = keyState;
        }
    }
}