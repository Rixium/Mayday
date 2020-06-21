using System.Collections.Generic;
using System.IO;
using Mayday.Game.Gameplay.Items;
using Newtonsoft.Json;

namespace Mayday.Editor.Loaders
{
    public class ItemsLoader : IItemsLoader
    {
        public Dictionary<string, Item> Items { get; set; }

        public ItemsLoader()
        {
            var read = File.ReadAllText("..\\..\\..\\src\\Mayday.Game\\Content\\Data\\ItemData.json");
            Items = JsonConvert.DeserializeObject<Dictionary<string, Item>>(read);
        }
    }
}