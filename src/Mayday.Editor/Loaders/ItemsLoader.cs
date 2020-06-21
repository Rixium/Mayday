using System.Collections.Generic;
using System.IO;
using Mayday.Game.Gameplay.Items;
using Newtonsoft.Json;

namespace Mayday.Editor.Loaders
{
    public class ItemsLoader : IItemsLoader
    {
        private string _filePath = "..\\..\\..\\src\\Mayday.Game\\Content\\Data\\ItemData.json";

        public Dictionary<string, Item> Items { get; set; }

        public void SetItems(Dictionary<string, Item> items)
        {
            Items = items;
        }

        public void Save()
        {
            foreach (var item in Items)
            {
                if (item.Value.TileType != null && item.Value.TileType.Equals("None"))
                    item.Value.TileType = null;
                if (item.Value.WorldObjectType != null && item.Value.WorldObjectType.Equals("None"))
                    item.Value.WorldObjectType = null;
            }

            var toText = JsonConvert.SerializeObject(Items, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            File.WriteAllText(_filePath, toText);
        }

        public ItemsLoader()
        {
            var read = File.ReadAllText(_filePath);
            Items = JsonConvert.DeserializeObject<Dictionary<string, Item>>(read);
        }
    }
}