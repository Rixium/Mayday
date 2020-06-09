using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemDropperComponent : IComponent
    {
        public IEntity Entity { get; set; }
        public Action<ItemDrop> ItemDrop { get; set; }
        
        private readonly int _itemDropId;

        public ItemDropperComponent(int itemDropId)
        {
            _itemDropId = itemDropId;
        }
        
        public void OnAddedToEntity()
        {
            Entity.Destroy += (entity) => Drop();
        }

        private void Drop()
        {
            var itemData = GetItemData(_itemDropId);

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

        private static Item GetItemData(int itemId) =>
            ContentChest.ItemData.ContainsKey(itemId) 
                ? ContentChest.ItemData[itemId] : null;
    }
}