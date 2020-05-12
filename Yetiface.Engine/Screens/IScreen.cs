using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.ECS;
using Yetiface.Engine.ECS.Components;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Graphics.Renderers;
using Yetiface.Engine.UI;
using IUpdateable = Yetiface.Engine.ECS.Components.Updateables.IUpdateable;

namespace Yetiface.Engine.Screens
{
    public interface IScreen
    {
        /// <summary>
        /// Responsible for rendering all of the screen's renderable components.
        /// </summary>
        IRenderer Renderer { get; set; }
        
        /// <summary>
        /// Holds the complete list of renderable components so the renderers can make use of them.
        /// </summary>
        IList<IRenderable> Renderables { get; set; }
                
        // TODO We don't want to store all updateables in a single component list
        // TODO as ideally we want specific lists for specific types of updateables for
        // TODO cpu caching. But we'll sort it later. !!!!!!!!!!!!!!!!!!!!IMPORTANT HIGH PRIORITY U NO
        IList<IUpdateable> Updateables { get; set; }
        
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
        /// Creates a new entity with a given position.
        /// </summary>
        /// <returns>A new entity ready to go.</returns>
        IEntity CreateEntity(Vector2 position);
        
        /// <summary>
        /// Creates a new entity with the default position of 0, 0.
        /// </summary>
        /// <returns>A new entity ready to go.</returns>
        IEntity CreateEntity();

        /// <summary>
        /// Add an entity to the screen.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A reference to the entity.</returns>
        IEntity AddEntity(IEntity entity);

        /// <summary>
        /// Pass a component here and then the relevant system will pick it up and keep track of it.
        /// </summary>
        /// <param name="component">The component to add to the systems.</param>
        void AddComponentToSystems(IComponent component);
    }
}