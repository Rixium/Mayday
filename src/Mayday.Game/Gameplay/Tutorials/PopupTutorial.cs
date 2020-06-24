using System.Diagnostics;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Tutorials
{
    public class PopupTutorial<T> : Tutorial<T> where T : IEntity
    {

        public PopupTutorial(TutorialDefinition tutorialDefinition) : base(tutorialDefinition)
        {
            TriggerOnce = true;
        }

        protected override void Show(T obj)
        {
            var entityPosition = obj.Position;
            Debug.WriteLine($"IM HERE {entityPosition}");
        }

    }
}