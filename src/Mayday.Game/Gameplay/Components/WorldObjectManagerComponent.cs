using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public class WorldObjectManagerComponent : IComponent
    {
        public IEntity Entity { get; set; }
        public string WorldObjectType { get; set; }

        public WorldObjectManagerComponent(string worldObjectType)
        {
            WorldObjectType = worldObjectType;
        }

        public void OnAddedToEntity()
        {

        }
    }
}