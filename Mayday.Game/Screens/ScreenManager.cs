﻿using System.Collections.Generic;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    
    /// <inheritdoc />
    public class ScreenManager : IScreenManager
    {
        
        public IDictionary<string, IScreen> Screens { get; }
        
        private IScreen _activeScreen;
        
        private IScreen _nextScreen;

        public ScreenManager()
        {
            Screens = new Dictionary<string, IScreen>();
        }

        public void AddScreen(IScreen screen)
        {
            screen.ScreenManager = this;
            Screens.Add(screen.Name, screen);
            if (_activeScreen == null)
            {
                _activeScreen = screen;
            }
        }

        public void RemoveScreen(IScreen screen) => Screens.Remove(screen.Name);
        
        public void RemoveScreen(string screenName) => Screens.Remove(screenName);
        
        public void ChangeScreen(string screenName) => _activeScreen = Screens[screenName];

        public IScreen GetScreen(string screenName)
        {
            Screens.TryGetValue(screenName, out var screen);
            return screen;
        }

        public void Update() => _activeScreen?.Update();
        
        public void Draw()
        {
            GraphicsUtils.SpriteBatch.GraphicsDevice.Clear(_activeScreen?.BackgroundColor ?? Color.Black);
            GraphicsUtils.Begin();
            _activeScreen?.Draw();
            GraphicsUtils.End();
        }
    }
}