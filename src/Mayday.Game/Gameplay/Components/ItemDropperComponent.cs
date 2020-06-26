using System;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemDropperComponent : IComponent
    {
        public IEntity Entity { get; set; }
        public Action<IEntity, ItemDrop> ItemDrop { get; set; }
        public string ItemDropType { get; set; }
        
        public ItemDropperComponent(string itemDropType)
        {
            ItemDropType = itemDropType;
        }
        
        public void OnAddedToEntity()
        {
            Entity.Destroy += Drop;
            Entity.Destroy += OnEntityDestroyed;
        }

        public void OnEntityDestroyed(IEntity entity)
        {
            Entity.Destroy -= Drop;
            Entity.Destroy -= OnEntityDestroyed;
        }

        private void Drop(IEntity entity)
        {
            var itemData = GetItemData(ItemDropType);

            if (itemData == null) return;
            
            var itemDrop = new ItemDrop
            {
                Item = itemData,
                X = Entity.X,
                Y = Entity.Y,
                GameWorld = Entity.GameWorld
            };
            
            var moveComponent = itemDrop.AddComponent(new MoveComponent());
            itemDrop.AddComponent(new GravityComponent());
            moveComponent.XVelocity = Randomizer.Next(-10, 10);
            moveComponent.YVelocity = Randomizer.Next(0, 5);
            
            ItemDrop?.Invoke(entity, itemDrop);
        }

        private static Item GetItemData(string itemDropType) =>
            ContentChest.ItemData.ContainsKey(itemDropType) 
                ? ContentChest.ItemData[itemDropType] : null;
    }
}