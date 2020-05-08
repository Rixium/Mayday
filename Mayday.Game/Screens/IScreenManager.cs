using System.Collections.Generic;
using Mayday.Game.Screens.Transitions;

namespace Mayday.Game.Screens
{
    
    /// <summary>
    /// Keep track of a set of screens, and easily switch between the screens.
    /// </summary>
    public interface IScreenManager
    {
        
        /// <summary>
        /// Holds the collection of screen currently tracked by the screen manager.
        /// </summary>
        IDictionary<string, IScreen> Screens { get; }
        
        /// <summary>
        /// The screen transition can be used by the screen manager on transitioning from active to
        /// next screen.
        /// </summary>
        ITransition ScreenTransition { get; }

        /// <summary>
        /// Add a screen to the screen manager to track.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        void AddScreen(IScreen screen);
        
        /// <summary>
        /// Removes a given screen from the screen manager if it exists.
        /// </summary>
        /// <param name="screen">The screen to remove.</param>
        void RemoveScreen(IScreen screen);
        
        /// <summary>
        /// Removes a given screen by screen name from the screen manager if it exists.
        /// </summary>
        /// <param name="screenName">The name of the screen to remove.</param>
        void RemoveScreen(string screenName);

        /// <summary>
        /// Set the next screen to the given screen name if it exists.
        /// Transitions will happen as defined by the user.
        /// </summary>
        /// <param name="screenName">The name of the screen to transition to.</param>
        /// <param name="shouldTransition">Whether or not a transition should be played on changing screen.</param>
        void ChangeScreen(string screenName, bool shouldTransition = true);

        /// <summary>
        /// Returns a screen by a given screen name tracked by the screen manager if it exists.
        /// </summary>
        /// <param name="screenName">The screen name of the screen.</param>
        /// <returns>The screen of the specified screen name.</returns>
        IScreen GetScreen(string screenName);

        /// <summary>
        /// Set the transition that should be played between screen changes.
        /// </summary>
        /// <param name="transition">The transition to play between screens.</param>
        void SetScreenTransition(ITransition transition);

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