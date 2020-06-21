using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.ViewModels
{
    public class TileViewViewModel
    {
        public TileProperties Tile { get; set; }

        public string Key { get; set; }

        private readonly ITileLoader _tileLoader;

        private ICommand _saveItemCommand;
        public ICommand SaveItemCommand => _saveItemCommand ?? new RelayCommand(SaveItem);

        public string TileName
        {
            get => Tile.Name;
            set
            {
                if (Tile.Name.Equals(value))
                    return;

                Tile.Name = value;
                OnPropertyChanged();
            }
        }

        public TileViewViewModel(string key, TileProperties tile, ITileLoader tileLoader)
        {
            Key = key;
            Tile = tile;
            _tileLoader = tileLoader;
        }

        private void SaveItem()
        {
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

            Navigator.ShowPage(new WorldObjectsManagerControl());
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}