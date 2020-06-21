using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class TileViewViewModel
    {
        public TileProperties Tile { get; set; }

        public string Key { get; set; }

        private readonly ITileLoader _tileLoader;
        private readonly IItemsLoader _itemLoader;

        private ICommand _saveItemCommand;
        public ICommand SaveItemCommand => _saveItemCommand ?? new RelayCommand(SaveItem);

        public string TileName
        {
            get => Tile.Name;
            set
            {
                if (Tile.Name != null && Tile.Name.Equals(value))
                    return;

                Tile.Name = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Item> Items => _itemLoader.Items.Values.AsEnumerable();
        public Item SelectedItemDrop { get; set; }

        public TileViewViewModel(string key, TileProperties tile, ITileLoader tileLoader, IItemsLoader itemLoader)
        {
            Key = key;
            Tile = tile;
            _tileLoader = tileLoader;
            _itemLoader = itemLoader;

            InitializeItemDropType();
        }


        private void InitializeItemDropType()
        {
            foreach (var item in Items)
            {
                if (item.ItemId.Equals(Tile.ItemDropType))
                {
                    SelectedItemDrop = item;
                    return;
                }
            }
        }

        private void SaveItem()
        {
            Tile.ItemDropType = SelectedItemDrop?.ItemId;

            var tiles = _tileLoader.Tiles;

            if (tiles.ContainsKey(Key))
            {
                if (Key.Equals(Tile.Name))
                {
                    tiles[Key] = Tile;
                }
                else
                {
                    tiles.Remove(Key);
                    tiles.Add(Tile.Name, Tile);
                }
            }
            else
            {
                tiles.Add(Tile.Name, Tile);
            }

            _tileLoader.SetTiles(tiles);
            _tileLoader.Save();

            Navigator.ShowPage(new TilesManagerControl());
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}