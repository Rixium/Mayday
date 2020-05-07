using System.Collections.Generic;

namespace Mayday.Game.Screens
{
    
    /// <summary>
    /// Keep track of a set of screens, and easily switch between the screens.
    /// </summary>
    public class ScreenManager : IScreenManager
    {

        /// <summary>
        /// Keep hold of all the screens in the screen manager, by their screen name.
        /// </summary>
        private Dictionary<string, IScreen> _screens;
        
        /// <summary>
        /// The current screen to draw and update.
        /// </summary>
        private IScreen _activeScreen;
        
        /// <summary>
        /// The next screen to transition to.
        /// </summary>
        private IScreen _nextScreen;

        /// <summary>
        /// Creates a new instance of the screen manager, and initializes the screen collection.
        /// </summary>
        public ScreenManager()
        {
            _screens = new Dictionary<string, IScreen>();
        }
        
        /// <summary>
        /// Add a screen to the screen manager to track.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        public void AddScreen(IScreen screen) => _screens.Add(screen.Name, screen);

        /// <summary>
        /// Removes a given screen from the screen manager if it exists.
        /// </summary>
        /// <param name="screen">The name of the screen to remove.</param>
        public void RemoveScreen(IScreen screen) => _screens.Remove(screen.Name);

        /// <summary>
        /// Set the next screen to the given screen name if it exists.
        /// Transitions will happen as defined by the user.
        /// </summary>
        /// <param name="screenName">The name of the screen to transition to.</param>
        public void ChangeScreen(string screenName) => _nextScreen = _screens[screenName];

        /// <summary>
        /// Returns a screen by a given screen name tracked by the screen manager if it exists.
        /// </summary>
        /// <param name="screenName">The screen name of the screen.</param>
        /// <returns>The screen of the specified screen name.</returns>
        public IScreen GetScreen(string screenName) => _screens[screenName];

        /// <summary>
        /// Updates the active screen, or updates the transition if the next screen is set.
        /// </summary>
        public void Update() => _activeScreen?.Update();
        
        /// <summary>
        /// Draws the active screen, or transition if the next screen is set.
        /// </summary>
        public void Draw() => _activeScreen?.Draw();
        
    }
}