using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.ECS;
using Yetiface.Engine.UI;

namespace Yetiface.Engine.Screens
{
    public interface IScreen
    {
        
        bool IsDebug { get; set; }
        
        HashSet<IEntity> Entities { get; set; }
        
        IScreenManager ScreenManager { get; set; }

        /// <summary>
        /// Use this to set up each screen user interface.
        /// Add elements to the user interface.
        /// </summary>
        IUserInterface UserInterface { get; set; }
        
        Color BackgroundColor { get; set; }
        
        string Name { get; set; }
        
        /// <summary>
        /// Indicates if this has it's own logic to change screens,
        /// and a call to change screen should be ignored until a later time.
        /// While this is true, no screen change will happen.
        /// </summary>
        bool IsForced { get; }

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

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <returns>A new entity ready to go.</returns>
        IEntity CreateEntity(string entityName);

        /// <summary>
        /// Add an entity to the screen.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A reference to the entity.</returns>
        IEntity AddEntity(IEntity entity);

    }
}