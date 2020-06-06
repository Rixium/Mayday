using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPickerComponent : IComponent
    {
        public IEntity Entity { get; set; }
        
        public void Update()
        {
            var listOfItems = new List<IEntity>(Entity.GameWorld.WorldItems);
            var toRemove = new List<IEntity>();
            foreach (var entity in listOfItems)
            {
                if (entity.GetType() != typeof(ItemDrop)) continue;
                var item = (ItemDrop) entity;
                if (CloseEnoughToGet(item))
                {
                    var inventoryComponent = Entity.GetComponent<InventoryComponent>();
                    inventoryComponent.AddItemToInventory(item.Item);
                    toRemove.Add(entity);
                }
            }

            listOfItems.RemoveAll(m => toRemove.Contains(m));
            Entity.GameWorld.WorldItems = listOfItems;
        }

        private bool CloseEnoughToGet(IEntity item) => item.GetBounds().Intersects(Entity.GetBounds());

        public void OnAddedToPlayer()
        {
            
        }
    }
}