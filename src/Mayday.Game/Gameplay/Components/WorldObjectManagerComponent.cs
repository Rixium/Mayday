using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;

namespace Mayday.Game.Gameplay.Components
{
    public class WorldObjectManagerComponent : IComponent
    {
        public IEntity Entity { get; set; }
        public WorldObjectType WorldObjectType { get; set; }

        public WorldObjectManagerComponent(WorldObjectType worldObjectType)
        {
            WorldObjectType = worldObjectType;
        }

        public void OnAddedToEntity()
        {

        }
    }
}