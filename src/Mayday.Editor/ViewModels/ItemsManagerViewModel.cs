using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class ItemsManagerViewModel
    {

        public Action<string, Item> OnUpdateItem;

        private readonly IItemsLoader _itemsLoader;

        public string MainText { get; set; } = "Hello World!";

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                UpdateSelectedItem();
            }
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand => _updateCommand ?? new RelayCommand(UpdateSelectedItem);

        private ICommand _newCommand;
        public ICommand NewCommand => _newCommand ?? new RelayCommand(() =>
            Navigator.ShowPage(new ItemViewControl("", new Item())));

        public IEnumerable<Item> Items => _itemsLoader.Items.Values.AsEnumerable();

        public ItemsManagerViewModel(IItemsLoader itemsLoader)
        {
            _itemsLoader = itemsLoader;
        }

        public void UpdateSelectedItem()
        {
            if (SelectedItem == null) return;

            var dictionarySelection =
                _itemsLoader.Items.FirstOrDefault(m => m.Value.ItemId.Equals(SelectedItem.ItemId));

            if (dictionarySelection.Key == null) return;

            OnUpdateItem?.Invoke(dictionarySelection.Key, dictionarySelection.Value);
        }

    }
}