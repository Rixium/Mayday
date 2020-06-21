using System.Collections.Generic;
using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.Loaders
{
    public interface IWorldObjectLoader
    {
        Dictionary<string, WorldObjectData> WorldObjects { get; set; }
        void Save();
        void SetWorldObjects(Dictionary<string, WorldObjectData> worldObjects);
    }
}