namespace Mayday.Game.Screens
{
    
    /// <summary>
    /// Keep track of a set of screens, and easily switch between the screens.
    /// </summary>
    public interface IScreenManager
    {
        
        /// <summary>
        /// Add a screen to the screen manager to track.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        void AddScreen(IScreen screen);
        
        /// <summary>
        /// Removes a given screen from the screen manager if it exists.
        /// </summary>
        /// <param name="screen">The name of the screen to remove.</param>
        void RemoveScreen(IScreen screen);

        /// <summary>
        /// Set the next screen to the given screen name if it exists.
        /// Transitions will happen as defined by the user.
        /// </summary>
        /// <param name="screenName">The name of the screen to transition to.</param>
        void ChangeScreen(string screenName);

        /// <summary>
        /// Returns a screen by a given screen name tracked by the screen manager if it exists.
        /// </summary>
        /// <param name="screenName">The screen name of the screen.</param>
        /// <returns>The screen of the specified screen name.</returns>
        IScreen GetScreen(string screenName);

        /// <summary>
        /// Updates the active screen, or updates the transition if the next screen is set.
        /// </summary>
        void Update();

        /// <summary>
        /// Draws the active screen, or transition if the next screen is set.
        /// </summary>
        void Draw();

    }
}