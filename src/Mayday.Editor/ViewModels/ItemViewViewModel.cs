using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Mayday.Editor.Loaders;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class ItemViewViewModel : INotifyPropertyChanged
    {
        private readonly IWorldObjectLoader _worldObjectLoader;
        private readonly ITileLoader _tileLoader;

        public string[] ItemUseTypes { get; set; } =
        {
            "None",
            "Tile",
            "World Object",
            "Tool"
        };

        private int _selectedItemUseTypeIndex;

        public string SelectedItemUseType
        {
            get => ItemUseTypes[_selectedItemUseTypeIndex];
            set
            {
                var newIndex = Array.IndexOf(ItemUseTypes, value);

                if (newIndex == _selectedItemUseTypeIndex) return;

                _selectedItemUseTypeIndex = newIndex;
                
                OnPropertyChanged(nameof(ItemUseTypeIsTile));
                OnPropertyChanged(nameof(ItemUseTypeIsWorldObject));
            }
        }

        public Item Item { get; }

        public IEnumerable<TileProperties> TileTypes =>
            _tileLoader.Tiles.Values.AsEnumerable();

        public IEnumerable<WorldObjectData> WorldObjectTypes =>
            _worldObjectLoader.WorldObjects.Values.AsEnumerable();

        public bool ItemUseTypeIsTile => SelectedItemUseType.Equals("Tile");
        public bool ItemUseTypeIsWorldObject => SelectedItemUseType.Equals("World Object");
        public bool ItemUseTypeIsTool => SelectedItemUseType.Equals("Tool");

        public WorldObjectData SelectedWorldObjectType { get; set; }

        public TileProperties SelectedTileType { get; set; }

        public ItemViewViewModel(Item item, IWorldObjectLoader worldObjectLoader, ITileLoader tileLoader)
        {
            _worldObjectLoader = worldObjectLoader;
            _tileLoader = tileLoader;

            Item = item;

            InitializeItemTypeIndex();

            SelectedWorldObjectType = WorldObjectTypes.FirstOrDefault(m => m.Name == (Item.WorldObjectType));
            SelectedTileType = TileTypes.FirstOrDefault(m => m.Name == (Item.TileType));
        }

        private void InitializeItemTypeIndex()
        {
            if (Item.TileType != "None")
            {
                SelectedItemUseType = "Tile";
            }
            else if (Item.WorldObjectType != "None")
            {
                SelectedItemUseType = "World Object";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}