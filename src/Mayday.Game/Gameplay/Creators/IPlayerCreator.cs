using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Creators
{
    public interface IPlayerCreator
    {
        public IPlayerCreationResult CreateHostPlayer(IEntity entity);
        public IPlayerCreationResult CreateClientPlayer(IEntity entity);
    }
}