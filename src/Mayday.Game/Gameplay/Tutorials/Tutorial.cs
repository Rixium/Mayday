using System.Diagnostics;

namespace Mayday.Game.Gameplay.Tutorials
{
    public class Tutorial<T> : ITutorial where T : class
    {

        private readonly TutorialDefinition _tutorialDefinition;

        public Tutorial(TutorialDefinition tutorialDefinition)
        {
            _tutorialDefinition = tutorialDefinition;
        }

        public void Trigger(T obj)
        {
            Debug.WriteLine(_tutorialDefinition.Text);
        }

    }
}