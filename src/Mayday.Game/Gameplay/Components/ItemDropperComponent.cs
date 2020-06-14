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
        public Action<ItemDrop> ItemDrop { get; set; }
        public ItemType ItemDropType { get; set; }
        
        public ItemDropperComponent(ItemType itemDropType)
        {
            ItemDropType = itemDropType;
        }
        
        public void OnAddedToEntity()
        {
            Entity.Destroy += (entity) => Drop();
        }

        private void Drop()
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
            
            ItemDrop?.Invoke(itemDrop);
        }

        private static Item GetItemData(ItemType itemDropType) =>
            ContentChest.ItemData.ContainsKey(itemDropType) 
                ? ContentChest.ItemData[itemDropType] : null;
    }
}