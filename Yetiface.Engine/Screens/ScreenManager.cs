using System.Collections.Generic;
using Yetiface.Engine.Screens.Transitions;

namespace Yetiface.Engine.Screens
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

        public T AddScreen<T>(T screen) where T : IScreen
        {
            screen.ScreenManager = this;
            Screens.Add(screen.Name, screen);

            if (_activeScreen != null)
            {
                if (_nextScreen == null)
                    _nextScreen = screen;

                return screen;
            }

            _activeScreen = screen;
            _activeScreen.Awake();

            ScreenTransition?.SetTransitionDirection(TransitionDirection.In);

            return screen;
        }

        public void RemoveScreen(IScreen screen) => Screens.Remove(screen.Name);

        public void RemoveScreen(string screenName) => Screens.Remove(screenName);

        public void ChangeScreen(string screenName, bool transition = true)
        {
            _nextScreen = Screens[screenName];

            if (_activeScreen.IsForced) return;

            if (transition)
                ScreenTransition?.SetTransitionDirection(TransitionDirection.Out);
            else
            {
                _activeScreen?.Finish();
                _activeScreen = _nextScreen;
                _nextScreen = null;
            }
        }

        public void NextScreen()
        {
            if (_nextScreen != null)
                ChangeScreen(_nextScreen.Name);
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

        private void TransitionInComplete() => _activeScreen?.Begin();

        private void TransitionOutComplete()
        {
            _activeScreen?.Finish();

            if (_nextScreen != null)
            {
                _activeScreen = _nextScreen;
                _activeScreen.Awake();
            }

            _nextScreen = null;

            ScreenTransition?.SetTransitionDirection(TransitionDirection.In);
        }

        public void Update()
        {
            _activeScreen?.Update();
            ScreenTransition?.Update();
        }

        public void Draw()
        {
            _activeScreen?.Draw();
            ScreenTransition?.Draw();
        }

        public IScreen GetActiveScreen() => _activeScreen;
    }
}