using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class ItemViewViewModel : INotifyPropertyChanged
    {
        public string[] ItemUseTypes { get; set; } =
        {
            "None",
            "Tile",
            "World Object"
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

        public Item Item { get; set; }

        public bool ItemUseTypeIsTile => SelectedItemUseType.Equals("Tile");
        public bool ItemUseTypeIsWorldObject => SelectedItemUseType.Equals("World Object");

        public ItemViewViewModel(Item item)
        {
            Item = item;

            InitializeItemTypeIndex();
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