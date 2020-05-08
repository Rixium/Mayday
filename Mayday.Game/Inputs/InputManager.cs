using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Mayday.Game.Inputs
{

    public class JsonBinding
    {
        public string BindingName { get; set; }
        public Keys Key { get; set; } = Keys.None;
        
        public int GamePadIndex { get; set; }
        public Buttons GamePadButton { get; set; }
        
        public MouseButton MouseButton { get; set; } = MouseButton.None;
    }

    /// <summary>
    /// MonoGame Mouse implementation has a property for each mouse button, and they don't use an enum,
    /// therefore a user can't select which they want. Using this so they can, and then we'll decide
    /// when implementing MouseButtonBinding how to deal with the different MouseButtons below.
    /// </summary>
    public enum MouseButton
    {
        None,
        Left,
        Right
    }
    
    public class InputManager : IInputManager
    {
        
        public HashSet<IInputBinding> InputBindings { get; set; }
        
        public Action OnInputEvent { get; set; }

        public InputManager()
        {
            InputBindings = new HashSet<IInputBinding>();
        }

        public void Initialize(string bindings)
        {
            var jsonBindings = JsonConvert.DeserializeObject<JsonBinding[]>(bindings);

            foreach (var binding in jsonBindings)
            {
                if (binding.Key != Keys.None) 
                    AddBinding(binding.BindingName, new KeyInputBinding(binding.Key));
                if (binding.GamePadButton != 0)
                    AddBinding(binding.BindingName, new GamePadInputBinding(binding.GamePadIndex, binding.GamePadButton));
                if(binding.MouseButton != MouseButton.None)
                    AddBinding(binding.BindingName, new MouseButtonBinding(binding.MouseButton));
            }
        }

        public void AddBinding(string name, IInputBinding binding)
        {
            binding.Name = name;
            InputBindings.Add(binding);
        }

        public void Update()
        {
            foreach (var binding in InputBindings) 
                binding.Update();
        }

        public void RegisterInputEvent(string bindingName, Action callback, InputEventType eventType = InputEventType.Pressed)
        {
            foreach (var binding in InputBindings.Where(binding => binding.Name.Equals(bindingName)))
                RegisterInputEvent(binding, callback, eventType);
        }

        public void RegisterInputEvent(IInputBinding binding, Action callback, InputEventType eventType = InputEventType.Pressed)
        {
            switch (eventType)
            {
                case InputEventType.Pressed:
                    binding.OnPressed.Add(callback);
                    break;
                case InputEventType.Released:
                    binding.OnReleased.Add(callback);
                    break;
                case InputEventType.Held:
                    binding.OnHeld.Add(callback);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        public void DeRegisterInputEvent(string bindingName, Action callback, InputEventType eventType = InputEventType.Pressed)
        {
            foreach (var binding in InputBindings.Where(binding => binding.Name.Equals(bindingName)))
                DeRegisterInputEvent(binding, callback, eventType);
        }

        /// <summary>
        /// Remove a certain callback from an input binding.
        /// Important as all callbacks will be called through Invoke regardless of whether or not the screen is
        /// currently active. We don't want that.
        /// </summary>
        /// <param name="binding">The input binding of the callback.</param>
        /// <param name="callback">The callback to remove.</param>
        /// <param name="eventType">The type of event the callback is mapped to.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the event type doesn't exist.</exception>
        public void DeRegisterInputEvent(IInputBinding binding, Action callback, InputEventType eventType = InputEventType.Pressed)
        {
            switch (eventType)
            {
                case InputEventType.Pressed:
                    binding.OnPressed.Remove(callback);
                    break;
                case InputEventType.Released:
                    binding.OnReleased.Remove(callback);
                    break;
                case InputEventType.Held:
                    binding.OnHeld.Remove(callback);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }
    }
}