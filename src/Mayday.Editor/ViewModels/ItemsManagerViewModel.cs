using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Loaders;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class ItemsManagerViewModel
    {
        private readonly IItemsLoader _itemsLoader;

        public string MainText { get; set; } = "Hello World!";
        public Item SelectedItem { get; set; }

        private ICommand _updateCommand;
        public ICommand UpdateCommand => _updateCommand ?? new RelayCommand(UpdateSelectedItem);

        public IEnumerable<Item> Items => _itemsLoader.Items.Values.AsEnumerable();

        public ItemsManagerViewModel(IItemsLoader itemsLoader)
        {
            _itemsLoader = itemsLoader;
        }

        public void UpdateSelectedItem()
        {

        }

    }
}