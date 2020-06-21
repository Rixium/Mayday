using System.Collections.Generic;
using System.IO;
using Mayday.Game.Gameplay.Data;
using Newtonsoft.Json;

namespace Mayday.Editor.Loaders
{
    public class WorldObjectLoader : IWorldObjectLoader
    {
        private string _filePath = "..\\..\\..\\src\\Mayday.Game\\Content\\Data\\WorldObjectData.json";
        public Dictionary<string, WorldObjectData> WorldObjects { get; set; }

        public void Save()
        {
            var toText = JsonConvert.SerializeObject(WorldObjects, Formatting.Indented,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });

            File.WriteAllText(_filePath, toText);
        }

        public void SetWorldObjects(Dictionary<string, WorldObjectData> worldObjects) => WorldObjects = worldObjects;

        public WorldObjectLoader()
        {
            var read = File.ReadAllText(_filePath);
            WorldObjects = JsonConvert.DeserializeObject<Dictionary<string, WorldObjectData>>(read);
        }
    }
}