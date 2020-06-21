using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Loaders;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Items;
using Newtonsoft.Json;

namespace Mayday.Editor.ViewModels
{
    public class ItemViewViewModel : INotifyPropertyChanged
    {
        private readonly IWorldObjectLoader _worldObjectLoader;
        private readonly ITileLoader _tileLoader;
        private readonly IItemsLoader _itemsLoader;

        private ICommand _saveItemCommand;
        public ICommand SaveItemCommand => _saveItemCommand ?? new RelayCommand(SaveItem);

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

        public string Key { get; }
        public Item Item { get; }

        public string ItemId
        {
            get => Item.ItemId;
            set
            {
                if (Item.ItemId.Equals(value))
                    return;

                Item.ItemId = value;
                OnPropertyChanged();
            }
        }

        public string ItemName
        {
            get => Item.Name;
            set
            {
                if (Item.Name.Equals(value))
                    return;

                Item.Name = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<TileProperties> TileTypes =>
            _tileLoader.Tiles.Values.AsEnumerable();

        public IEnumerable<WorldObjectData> WorldObjectTypes =>
            _worldObjectLoader.WorldObjects.Values.AsEnumerable();

        public bool ItemUseTypeIsTile => SelectedItemUseType.Equals("Tile");
        public bool ItemUseTypeIsWorldObject => SelectedItemUseType.Equals("World Object");
        public bool ItemUseTypeIsTool => SelectedItemUseType.Equals("Tool");

        public WorldObjectData SelectedWorldObjectType { get; set; }

        public TileProperties SelectedTileType { get; set; }

        public ItemViewViewModel(string key, Item item, IWorldObjectLoader worldObjectLoader, ITileLoader tileLoader, IItemsLoader itemsLoader)
        {
            _worldObjectLoader = worldObjectLoader;
            _tileLoader = tileLoader;
            _itemsLoader = itemsLoader;

            Key = key;
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

        private void SaveItem()
        {
            ValidateItemUseCase();
            var items = _itemsLoader.Items;

            if (items.ContainsKey(Key))
            {
                if (Key.Equals(Item.ItemId))
                {
                    items[Key] = Item;
                }
                else
                {
                    items.Remove(Key);
                    items.Add(Item.ItemId, Item);
                }
            }
            else
            {
                items.Add(Item.ItemId, Item);
            }

            _itemsLoader.SetItems(items);
            _itemsLoader.Save();
        }

        private void ValidateItemUseCase()
        {
            switch (SelectedItemUseType)
            {
                case "None":
                    Item.TileType = "None";
                    Item.WorldObjectType = "None";
                    break;
                case "Tile":
                    Item.WorldObjectType = "None";
                    Item.TileType = SelectedTileType.Name;
                    break;
                case "World Object":
                    Item.TileType = "None";
                    Item.WorldObjectType = SelectedWorldObjectType.Name;
                    break;
                case "Tool":
                    Item.TileType = "None";
                    Item.WorldObjectType = "None";
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}