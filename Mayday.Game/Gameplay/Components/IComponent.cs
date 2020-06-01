using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public interface IComponent
    {

        IPlayer Player { get; set; }
        void Update();
        void OnAddedToPlayer();
    }
}