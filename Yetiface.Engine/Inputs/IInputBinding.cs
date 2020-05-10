using System;
using System.Collections.Generic;

namespace Yetiface.Engine.Inputs
{

    public enum InputEventType
    {
        Pressed,
        Released,
        Held
    }
    
    public interface IInputBinding
    {
        
        string Name { get; set; }
        IList<Action> OnPressed { get; set; }
        IList<Action> OnReleased { get; set; }
        IList<Action> OnHeld { get; set; }

        void Update();
    }
}