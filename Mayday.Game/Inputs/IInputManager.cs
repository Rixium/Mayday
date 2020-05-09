using System;
using System.Collections.Generic;

namespace Mayday.Game.Inputs
{
    public interface IInputManager
    {
        HashSet<IInputBinding> InputBindings { get; set; }

        void Initialize(string bindings);

        void AddBinding(string name, IInputBinding binding);

        void Update();

        void RegisterInputEvent(string bindingName, Action callback, InputEventType eventType = InputEventType.Pressed);

        void RegisterInputEvent(IInputBinding binding, Action callback,
            InputEventType eventType = InputEventType.Pressed);

        void DeRegisterInputEvent(string binding, Action callback, InputEventType eventType = InputEventType.Pressed);

        void DeRegisterInputEvent(IInputBinding binding, Action callback,
            InputEventType eventType = InputEventType.Pressed);
    }
}