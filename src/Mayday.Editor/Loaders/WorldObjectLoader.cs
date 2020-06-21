using System.Collections.Generic;
using System.IO;
using Mayday.Game.Gameplay.Data;
using Newtonsoft.Json;

namespace Mayday.Editor.Loaders
{
    public class WorldObjectLoader : IWorldObjectLoader
    {
        public Dictionary<string, WorldObjectData> WorldObjects { get; set; }

        public WorldObjectLoader()
        {
            var read = File.ReadAllText("..\\..\\..\\src\\Mayday.Game\\Content\\Data\\WorldObjectData.json");
            WorldObjects = JsonConvert.DeserializeObject<Dictionary<string, WorldObjectData>>(read);
        }
    }
}