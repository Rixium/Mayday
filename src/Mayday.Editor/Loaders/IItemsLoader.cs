using System.Collections.Generic;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.Loaders
{
    public interface IItemsLoader
    {

        Dictionary<string, Item> Items { get; set; }

    }
}