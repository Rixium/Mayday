using System;

namespace Mayday.Game.Gameplay.Tutorials
{
    public abstract class Tutorial<T> : ITutorial
    {

        public bool TriggerOnce { get; set; }
        private readonly TutorialDefinition _tutorialDefinition;
        private bool _triggered;
        public Action<T> Trigger { get; set; }

        public Tutorial(TutorialDefinition tutorialDefinition)
        {
            _tutorialDefinition = tutorialDefinition;
            Trigger += OnTrigger;
        }

        private void OnTrigger(T obj) {

            if(TriggerOnce && _triggered)
            {
                return;
            }

            _triggered = true;

            Trigger -= OnTrigger;

            Show(obj);
        }

        protected abstract void Show(T obj);

        public Action Triggered { get; set; }
    }
}