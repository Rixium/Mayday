using System.Collections.Generic;
using System.IO;
using Mayday.Game.Gameplay.Data;
using Newtonsoft.Json;

namespace Mayday.Editor.Loaders
{
    public class TileLoader : ITileLoader
    {
        private string _filePath = "..\\..\\..\\src\\Mayday.Game\\Content\\Data\\TileProperties.json";
        
        public Dictionary<string, TileProperties> Tiles { get; set; }
        public void Save()
        {
            var toText = JsonConvert.SerializeObject(Tiles, Formatting.Indented,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });

            File.WriteAllText(_filePath, toText);
        }

        public void SetTiles(Dictionary<string, TileProperties> tiles) => Tiles = tiles;

        public TileLoader()
        {
            var read = File.ReadAllText(_filePath);
            Tiles = JsonConvert.DeserializeObject<Dictionary<string, TileProperties>>(read);
        }
    }
}