using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.ViewModels
{
    public class TilesManagerViewModel
    {
        public Action<string, TileProperties> OnUpdateTile;

        private readonly ITileLoader _tileLoader;

        public string MainText { get; set; } = "Hello World!";

        private TileProperties _selectedTile;
        public TileProperties SelectedTile
        {
            get => _selectedTile;
            set
            {
                _selectedTile = value;
                UpdateSelectedTile();
            }
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand => _updateCommand ?? new RelayCommand(UpdateSelectedTile);

        private ICommand _newCommand;
        public ICommand NewCommand => _newCommand ?? new RelayCommand(() =>
            Navigator.ShowPage(new TileViewControl("", new TileProperties())));

        public IEnumerable<TileProperties> Tiles => _tileLoader.Tiles.Values.AsEnumerable();

        public TilesManagerViewModel(ITileLoader tileLoader)
        {
            _tileLoader = tileLoader;
        }

        public void UpdateSelectedTile()
        {
            if (SelectedTile == null) return;

            var dictionarySelection =
                _tileLoader.Tiles.FirstOrDefault(m => m.Value.Name.Equals(SelectedTile.Name));

            if (dictionarySelection.Key == null) return;

            OnUpdateTile?.Invoke(dictionarySelection.Key, dictionarySelection.Value);
        }

    }
}