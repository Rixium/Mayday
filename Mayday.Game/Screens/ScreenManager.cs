using System.Collections.Generic;
using Mayday.Game.Screens.Transitions;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    
    /// <inheritdoc />
    public class ScreenManager : IScreenManager
    {
        
        public IDictionary<string, IScreen> Screens { get; }

        public ITransition ScreenTransition { get; private set; }

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

            if (_activeScreen != null) return;
            
            _activeScreen = screen;
            ScreenTransition.SetTransitionDirection(TransitionDirection.In);
        }

        public void RemoveScreen(IScreen screen) => Screens.Remove(screen.Name);
        
        public void RemoveScreen(string screenName) => Screens.Remove(screenName);
        
        public void ChangeScreen(string screenName)
        {
            _nextScreen = Screens[screenName];
            ScreenTransition.SetTransitionDirection(TransitionDirection.Out);
        }

        public IScreen GetScreen(string screenName)
        {
            Screens.TryGetValue(screenName, out var screen);
            return screen;
        }

        public void SetScreenTransition(ITransition transition)
        {
            ScreenTransition = transition;
            ScreenTransition.OnTransitionInComplete += TransitionInComplete;
            ScreenTransition.OnTransitionOutComplete += TransitionOutComplete;
        }

        private void TransitionInComplete()
        {
            _activeScreen?.Initialize();
        }

        private void TransitionOutComplete()
        {
            if(_nextScreen != null)
                _activeScreen = _nextScreen;
            
            ScreenTransition.SetTransitionDirection(TransitionDirection.In);
        }

        public void Update()
        {
            _activeScreen?.Update();
            ScreenTransition?.Update();
        }
        
        public void Draw()
        {
            GraphicsUtils.SpriteBatch.GraphicsDevice.Clear(_activeScreen?.BackgroundColor ?? Color.Black);
            
            GraphicsUtils.Begin();
            _activeScreen?.Draw();
            ScreenTransition?.Draw();
            GraphicsUtils.End();
        }
    }
}