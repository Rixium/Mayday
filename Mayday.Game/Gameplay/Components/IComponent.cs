using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public interface IComponent
    {

        IEntity Entity { get; set; }
        void Update();
        void OnAddedToPlayer();
    }
}