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
            var toText = JsonConvert.SerializeObject(Items, Formatting.Indented);
            File.WriteAllText(_filePath, toText);
        }

        public ItemsLoader()
        {
            var read = File.ReadAllText(_filePath);
            Items = JsonConvert.DeserializeObject<Dictionary<string, Item>>(read);
        }
    }
}