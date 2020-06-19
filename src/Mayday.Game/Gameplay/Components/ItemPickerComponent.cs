using System.Collections.Generic;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Microsoft.Xna.Framework.Audio;
using Yetiface.Engine;
using Yetiface.Engine.Optimization;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPickerComponent : IUpdateable
    {
        
        private InventoryComponent _inventoryComponent;
        private IWorldItemSet _worldItemSet;
        private IUpdateResolver<IEntity> _updateResolver;
        private double _lastPickup;
        private float _totalItemsLastSecond;

        public IEntity Entity { get; set; }

        public ItemPickerComponent(IWorldItemSet worldItemSet, IUpdateResolver<IEntity> updateResolver)
        {
            _worldItemSet = worldItemSet;
            _updateResolver = updateResolver;
        }
        
        public void Update()
        {
            var newList = new List<IEntity>();
            var pickedUp = false;

            if (Time.GameTime.TotalGameTime.TotalSeconds > _lastPickup + 1.0f)
            {
                _totalItemsLastSecond = 0;
            }
            
            foreach (var entity in _worldItemSet.GetItems())
            {
                if (!_updateResolver.ShouldUpdate(entity))
                {
                    newList.Add(entity);
                    continue;
                }

                if (!(entity is ItemDrop item)) 
                    continue;
                
                if (!CloseEnoughToGet(item))
                {
                    newList.Add(entity);
                    continue;
                }
                
                _inventoryComponent.AddItemToInventory(item.Item);
                pickedUp = true;
            }

            if (pickedUp)
            {
                _lastPickup = Time.GameTime.TotalGameTime.TotalSeconds;
                _totalItemsLastSecond++;
                YetiGame.ContentManager.Load<SoundEffect>("pickup").Play(0.5f, 1.0f / _totalItemsLastSecond, 0.0f);
            }

            _worldItemSet.Set(newList);
        }

        private bool CloseEnoughToGet(IEntity item) => item.GetCurrentBounds().Intersects(Entity.GetCurrentBounds());

        public void OnAddedToEntity()
        {
            _inventoryComponent = Entity.GetComponent<InventoryComponent>();   
        }
    }
}