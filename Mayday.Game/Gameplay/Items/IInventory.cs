using System;
using System.Collections.Generic;

namespace Mayday.Game.Gameplay.Items
{
    public interface IInventory
    {
        Action<IItem> ItemPickup { get; set; }
        Action InventoryChanged { get; set; }
        IList<IItemStack> ItemStacks { get; set; }
        int Slots { get; set; }
        bool AddItemToInventory(IItem item);
    }
}