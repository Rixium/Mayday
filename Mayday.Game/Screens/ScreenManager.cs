using System.Collections.Generic;

namespace Mayday.Game.Screens
{
    
    /// <inheritdoc />
    public class ScreenManager : IScreenManager
    {
        
        private readonly Dictionary<string, IScreen> _screens;
        
        private IScreen _activeScreen;
        
        private IScreen _nextScreen;

        public ScreenManager()
        {
            _screens = new Dictionary<string, IScreen>();
        }
        
        public void AddScreen(IScreen screen) => _screens.Add(screen.Name, screen);

        public void RemoveScreen(IScreen screen) => _screens.Remove(screen.Name);
        
        public void ChangeScreen(string screenName) => _nextScreen = _screens[screenName];

        public IScreen GetScreen(string screenName) => _screens[screenName];

        public void Update() => _activeScreen?.Update();
        
        public void Draw() => _activeScreen?.Draw();
        
    }
}