using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Screens;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPickerComponent : IUpdateable
    {
        private readonly GameScreen _gameScreen;
        private InventoryComponent _inventoryComponent;
        public IEntity Entity { get; set; }

        public ItemPickerComponent(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        public void Update()
        {
            var newList = new List<IEntity>();
            foreach (var entity in Entity.GameWorld.WorldItems)
            {
                if (!_gameScreen.Camera.Intersects(entity.GetBounds()))
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

            Entity.GameWorld.WorldItems = newList;
        }

        private bool CloseEnoughToGet(IEntity item) => item.GetBounds().Intersects(Entity.GetBounds());

        public void OnAddedToEntity()
        {
            _inventoryComponent = Entity.GetComponent<InventoryComponent>();   
        }
    }
}