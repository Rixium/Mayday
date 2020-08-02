using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.Items.Placers;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework.Audio;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPlacerComponent : IComponent
    {

        private double _lastPlaced;
        private Camera _camera;
        public IGameWorld GameWorld => Entity.GameWorld;
        public IEntity Entity { get; set; }
        public Action<IItem> ItemUsed { get; set; }
        public IItem SelectedItem { get; set; }
        public IList<IPlacer> ItemPlacers { get; set; }

        public ItemPlacerComponent(IList<IPlacer> placers, Camera camera)
        {
            ItemPlacers = placers;
            _camera = camera;
        }
        
        public void OnAddedToEntity()
        {
        }

        public void MouseDown(MouseButton button)
        {
            if (button != MouseButton.Left || Time.GameTime.TotalGameTime.TotalSeconds < _lastPlaced + 0.5f) 
                return;
            
            var itemPlacer = ItemPlacers.FirstOrDefault(x => x.PlacerFor(SelectedItem));

            if (itemPlacer == null) 
                return;
            
            var (i, y, _, _) = MouseState.Bounds(_camera.GetMatrix());
            var mouseTileX = i / GameWorld.TileSize;
            var mouseTileY = y / GameWorld.TileSize;

            if (!itemPlacer.Place(Entity, GameWorld, SelectedItem, mouseTileX, mouseTileY)) 
                return;
            
            YetiGame.ContentManager.Load<SoundEffect>("place").Play();
            ItemUsed?.Invoke(SelectedItem);
            _lastPlaced = Time.GameTime.TotalGameTime.TotalSeconds;
        }
        
        public void SetSelectedItem(IItem item) => SelectedItem = item;
    }
}