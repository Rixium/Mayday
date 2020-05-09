using Mayday.Game.UI;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    public interface IScreen
    {
        IScreenManager ScreenManager { get; set; }

        /// <summary>
        /// Use this to set up each screen user interface.
        /// Add elements to the user interface.
        /// </summary>
        IUserInterface UserInterface { get; set; }
        
        Color BackgroundColor { get; set; }
        
        string Name { get; set; }

        /// <summary>
        /// Called as soon as the screen is set as the active screen,
        /// Keep in mind the transition may still be playing.
        /// </summary>
        void Awake();
        
        /// <summary>
        /// Called as soon as the screen transition has ended, and the screen has all
        /// priority.
        /// </summary>
        void Begin();
        
        void Update();

        void Draw();

        /// <summary>
        /// Called as soon as the screen has finished transitioning out, use for cleanup.
        /// </summary>
        void Finish();
        
    }
}