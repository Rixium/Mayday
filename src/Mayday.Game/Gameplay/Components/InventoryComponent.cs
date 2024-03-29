using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.Components
{
    public class InventoryComponent : IComponent
    {
        public IEntity Entity { get; set; }
        private IList<IInventory> Inventories { get; set; }

        public void AddItemToInventory(IItem item)
        {
            foreach (var inventory in Inventories)
            {
                var result = inventory.AddItemToInventory(item);
                if (result)
                {
                    break;
                }
            }
        }

        public void OnAddedToEntity()
        {
        }

        public IInventory AddInventory(Inventory inventory)
        {
            if(Inventories == null)
                Inventories = new List<IInventory>();
            
            Inventories.Add(inventory);
            return inventory;
        }
    }
}