using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Creators
{
    public interface IPlayerCreator
    {
        public IEntity CreateHostPlayer(IEntity entity);
        public IEntity CreateClientPlayer(IEntity entity);
    }
}