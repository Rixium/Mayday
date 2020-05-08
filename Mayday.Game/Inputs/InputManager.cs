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

            InputBindings.Add(binding);
        }
        
    }
}