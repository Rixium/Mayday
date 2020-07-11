using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Gameplay.Components
{
    public class WorldObjectManagerComponent : IComponent
    {
        public IEntity Entity { get; set; }
        public string WorldObjectType { get; set; }
        public WorldObjectData WorldObjectData { get; set; }
        public Texture2D WorldObjectTexture { get; set; }

        public WorldObjectManagerComponent(string worldObjectType)
        {
            WorldObjectType = worldObjectType;
            WorldObjectData = ContentChest.WorldObjectData[worldObjectType];
            WorldObjectTexture = ContentChest.WorldObjectTextures[worldObjectType];
        }


        public void OnAddedToEntity()
        {
        }
    }
}