using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.ECS;
using Yetiface.Engine.ECS.Components;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Graphics.Renderers;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Utils;
using IUpdateable = Yetiface.Engine.ECS.Components.Updateables.IUpdateable;
using IUserInterface = Yetiface.Engine.UI.IUserInterface;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Yetiface.Engine.Screens
{
    public abstract class Screen : IScreen
    {
        public IRenderer Renderer { get; set; }
        public IList<IRenderable> Renderables { get; set; }
        
        // TODO We don't want to store all updateables in a single component list
        // TODO as ideally we want specific lists for specific types of updateables for
        // TODO cpu caching. But we'll sort it later. !!!!!!!!!!!!!!!!!!!!IMPORTANT HIGH PRIORITY U NO
        public IList<IUpdateable> Updateables { get; set; }
        public bool IsDebug { get; set; }
        public HashSet<IEntity> Entities { get; set; }
        public IScreenManager ScreenManager { get; set; }
        public IUserInterface UserInterface { get; set; }
        public Color BackgroundColor { get; set; }


        public KeyboardState LastKeyboardState;
        public Action<Keys> OnKeyDown;
        public Action<Keys> OnKeyUp;
        public Action<MouseButton> OnMouseDown;
        public Action<MouseButton> OnMouseUp;
        public Action<MouseButton> OnMouseHeld;

        public string Name { get; set; }

        public bool IsForced { get; protected set; }

        protected Screen(string name)
        {
            Renderer = new BasicRenderer(this);
            Name = name;
        }

        public virtual void Awake()
        {
            
        }

        public virtual void Begin()
        {
            
        }

        public virtual void Update()
        {
            var keyboardState = Keyboard.GetState();

            HandleKeyboard(keyboardState);
            HandleMouse();
            
            UserInterface?.Update();

            if (Updateables == null) return;
            foreach (var updateable in Updateables) 
                updateable.Update();

            LastKeyboardState = keyboardState;
        }

        private void HandleMouse()
        {
            if (UserInterface != null && UserInterface.MouseOver) return;
            
            if (MouseState.CurrentState.LeftButton == ButtonState.Pressed) 
                OnMouseDown?.Invoke(MouseButton.Left);

            if (MouseState.CurrentState.LeftButton == ButtonState.Pressed &&
                MouseState.LastState.LeftButton == ButtonState.Pressed) 
                OnMouseHeld?.Invoke(MouseButton.Left);

            if (MouseState.CurrentState.RightButton == ButtonState.Pressed) 
                OnMouseDown?.Invoke(MouseButton.Right);

            if (MouseState.CurrentState.RightButton == ButtonState.Pressed &&
                MouseState.LastState.RightButton == ButtonState.Pressed) 
                OnMouseHeld?.Invoke(MouseButton.Right);
        }

        private void HandleKeyboard(KeyboardState keyboardState)
        {
            foreach (var key in keyboardState.GetPressedKeys())
            {
                OnKeyDown?.Invoke(key);
            }
        }

        public void BeforeDraw()
        {
            GraphicsUtils.Instance.SpriteBatch.GraphicsDevice.Clear(BackgroundColor);
        }

        public void Draw()
        {
            BeforeDraw();

            GraphicsUtils.Instance.Begin();
            Renderer?.Draw();
            GraphicsUtils.Instance.End();
            
            RenderScreen();
            
            AfterDraw();
        }

        public abstract void RenderScreen();

        public void AfterDraw()
        {
            UserInterface?.Draw();
        }

        public virtual void Finish()
        {
            
        }

        /// <summary>
        /// Creates a new entity with a position of 0, 0.
        /// </summary>
        /// <returns>A newly created entity set up with the screen.</returns>
        public IEntity CreateEntity() => 
            CreateEntity(Vector2.Zero);
        
        /// <summary>
        /// Creates a new entity fully set up with the screen.
        /// </summary>
        /// <returns>Fully set up entity.</returns>
        public IEntity CreateEntity(Vector2 position) =>
            AddEntity(new Entity(this, position));


        /// <summary>
        /// Adds an entity to the screen and sets the entities screen to this screen.
        /// You probably want to use CreateEntity unless you are bringing entities over from another
        /// screen.
        /// </summary>
        /// <param name="entity">The entity to add to the screen.</param>
        /// <returns>Fully set up entity.</returns>
        public IEntity AddEntity(IEntity entity)
        {
            if (Entities == null)
                Entities = new HashSet<IEntity>();

            entity.Screen = this;
            Entities.Add(entity);
            return entity;
        }

        /// <summary>
        /// Adds a given component to the system. If the component is a renderable it adds it to the scenes
        /// renderables, which in turn are picked up by the renderer, to render during draw calls.
        /// </summary>
        /// <param name="component">The component to add to the systems.</param>
        public void AddComponentToSystems(IComponent component)
        {
            // Could be an IRenderable
            if (component is IRenderable renderable)
            {
                if(Renderables == null)
                    Renderables = new List<IRenderable>();
                Renderables.Add(renderable);
            }
            // Could also be an IUpdateable TOO!
            // Some components can implement both IRenderable and IUpdateable so check for both.
            // TODO Optimize? If we only add components at the start of the scene it probably doesn't matter.
            // Updateables probably wants to become some kind of type map though.
            if (component is IUpdateable updateable)
            {
                if (Updateables == null)
                    Updateables = new List<IUpdateable>();
                Updateables.Add(updateable);
            }
        }
        
    }
}