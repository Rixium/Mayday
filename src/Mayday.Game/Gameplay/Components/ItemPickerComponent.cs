using System.Collections.Generic;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Yetiface.Engine.Optimization;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPickerComponent : IUpdateable
    {
        
        private InventoryComponent _inventoryComponent;
        private IWorldItemSet _worldItemSet;
        private IUpdateResolver<IEntity> _updateResolver;

        public IEntity Entity { get; set; }

        public ItemPickerComponent(IWorldItemSet worldItemSet, IUpdateResolver<IEntity> updateResolver)
        {
            _worldItemSet = worldItemSet;
            _updateResolver = updateResolver;
        }
        
        public void Update()
        {
            var newList = new List<IEntity>();
            foreach (var entity in _worldItemSet.GetItems())
            {
                if (!_updateResolver.ShouldUpdate(entity))
                {
                    newList.Add(entity);
                    continue;
                }

                if (entity.GetType() != typeof(ItemDrop))
                {
                    newList.Add(entity);
                    continue;
                }
                
                var item = (ItemDrop) entity;
                if (!CloseEnoughToGet(item))
                {
                    newList.Add(entity);
                    continue;
                }
                
                _inventoryComponent.AddItemToInventory(item.Item);
            }

            _worldItemSet.Clear();
            _worldItemSet.Set(newList);
        }

        private bool CloseEnoughToGet(IEntity item) => item.GetBounds().Intersects(Entity.GetBounds());

        public void OnAddedToEntity()
        {
            _inventoryComponent = Entity.GetComponent<InventoryComponent>();   
        }
    }
}