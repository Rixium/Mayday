using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Tutorials;

namespace Mayday.Game.Gameplay.Components
{
    public class TutorialManagerComponent : IComponent
    {
        public IEntity Entity { get; set; }

        public IDictionary<string, ITutorial> Tutorials { get; set; }
            = new Dictionary<string, ITutorial>();

        public void OnAddedToEntity()
        {

        }

        public void AddTutorial(string name, ITutorial tutorial)
        {
            if (Tutorials.ContainsKey(name)) return;
            Tutorials.Add(name, tutorial);

            tutorial.Triggered += () => OnTutorialTriggered(name);
        }

        private void OnTutorialTriggered(string name)
        {
            Tutorials[name] = null;
            Tutorials.Remove(name);
        }

    }
}