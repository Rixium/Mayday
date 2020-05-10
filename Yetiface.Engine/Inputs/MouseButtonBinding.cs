using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Yetiface.Engine.Inputs
{
    public class MouseButtonBinding : IInputBinding
    {
        private readonly MouseButton _button;
        
        private MouseState _lastState;

        public string Name { get; set; }
        public IList<Action> OnPressed { get; set; }
        public IList<Action> OnReleased { get; set; }
        public IList<Action> OnHeld { get; set; }
        
        public MouseButtonBinding(MouseButton button)
        {
            _button = button;
            
            OnPressed = new List<Action>();
            OnReleased = new List<Action>();
            OnHeld = new List<Action>();
        }
        
        public void Update()
        {
            var mouseState = Mouse.GetState();
            
            if (_button == MouseButton.Left) ProcessMouse(mouseState.LeftButton, _lastState.LeftButton);
            if (_button == MouseButton.Right) ProcessMouse(mouseState.RightButton, _lastState.RightButton);

            _lastState = mouseState;
        }

        private void ProcessMouse(ButtonState buttonState, ButtonState lastState)
        {
            if (buttonState == ButtonState.Pressed && lastState == ButtonState.Released)
            {
                foreach (var action in OnPressed)
                    action?.Invoke();
            } else if (buttonState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                foreach (var action in OnReleased)
                    action?.Invoke();
            }
        }
        
    }
}