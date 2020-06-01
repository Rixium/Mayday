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
        private readonly string _gamePadAxis;

        public string Name { get; set; }
        public IList<Action> OnPressed { get; set; }
        public IList<Action> OnReleased { get; set; }
        public IList<Action> OnHeld { get; set; }

        public GamePadInputBinding(int gamePadIndex, Buttons gamePadButton, string gamePadAxis)
        {
            OnPressed = new List<Action>();
            OnReleased = new List<Action>();
            OnHeld = new List<Action>();
            
            _gamePadIndex = gamePadIndex;
            _gamePadButton = gamePadButton;
            _gamePadAxis = gamePadAxis;
        }
        
        public void Update()
        {
            var state = GamePad.GetState(_gamePadIndex);

            if(state.IsButtonDown(_gamePadButton) && _lastState.IsButtonUp(_gamePadButton) || AxisPressed(state, _gamePadAxis) && AxisReleased(_lastState, _gamePadAxis))
                foreach(var action in OnPressed)
                    action?.Invoke();
            else if(state.IsButtonUp(_gamePadButton) && _lastState.IsButtonDown(_gamePadButton) || AxisPressed(_lastState, _gamePadAxis)  && AxisReleased(state, _gamePadAxis))
                foreach(var action in OnReleased)
                    action?.Invoke();
            else if(state.IsButtonDown(_gamePadButton) && _lastState.IsButtonDown(_gamePadButton) || AxisPressed(_lastState, _gamePadAxis)  && AxisPressed(state, _gamePadAxis))
                foreach(var action in OnHeld)
                    action?.Invoke();

            _lastState = state;
        }

        private bool AxisPressed(GamePadState state, string gamePadAxis)
        {
            if (gamePadAxis == null) return false;
            
            if (gamePadAxis.Contains("-"))
            {
                if (gamePadAxis.Contains("X"))
                    return state.ThumbSticks.Left.X < -0.1f;

                return state.ThumbSticks.Left.Y < 0.1f;
            }

            if (gamePadAxis.Contains("X"))
                return state.ThumbSticks.Left.X > 0.1f;

            return state.ThumbSticks.Left.Y > 0.1f;
        }

        private bool AxisReleased(GamePadState lastState, string gamePadAxis)
        {
            if (gamePadAxis == null) return false;
            
            if (gamePadAxis.Contains("-"))
            {
                if (gamePadAxis.Contains("X"))
                    return lastState.ThumbSticks.Left.X > -0.1f;

                return lastState.ThumbSticks.Left.Y > 0.1f;
            }

            if (gamePadAxis.Contains("X"))
                return lastState.ThumbSticks.Left.X < 0.1f;

            return lastState.ThumbSticks.Left.Y < 0.1f;
        }
    }
}