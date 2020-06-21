using System.Collections.Generic;
using System.IO;
using Mayday.Game.Gameplay.Data;
using Newtonsoft.Json;

namespace Mayday.Editor.Loaders
{
    public class TileLoader : ITileLoader
    {
        public Dictionary<string, TileProperties> Tiles { get; set; }

        public TileLoader()
        {
            var read = File.ReadAllText("..\\..\\..\\src\\Mayday.Game\\Content\\Data\\TileProperties.json");
            Tiles = JsonConvert.DeserializeObject<Dictionary<string, TileProperties>>(read);
        }
    }
}