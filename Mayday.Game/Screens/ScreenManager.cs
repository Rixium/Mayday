using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public class ScreenManager : IScreenManager
    {

        private Dictionary<string, IScreen> _screens;
        private IScreen _activeScreen;
        private IScreen _nextScreen;
        
        public void AddScreen(IScreen screen) => _screens.Add(screen.Name, screen);

        public void RemoveScreen(IScreen screen) => _screens.Remove(screen.Name);

        public void ChangeScreen(string screenName) => _nextScreen = _screens[screenName];
        
        public void Update(float delta) => _activeScreen?.Update(delta);

        public void Draw(SpriteBatch spriteBatch) => _activeScreen?.Draw(spriteBatch);
        
    }
}