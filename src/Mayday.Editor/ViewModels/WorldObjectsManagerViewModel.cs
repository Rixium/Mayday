using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class WorldObjectsManagerViewModel
    {
        public Action<string, WorldObjectData> OnUpdateWorldObject;

        private WorldObjectData _selectedWorldObject;
        public WorldObjectData SelectedWorldObject
        {
            get => _selectedWorldObject;
            set
            {
                _selectedWorldObject = value;
                UpdateSelectedWorldObject();
            }
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand => _updateCommand ?? new RelayCommand(UpdateSelectedWorldObject);

        private ICommand _newCommand;
        private IWorldObjectLoader _worldObjectLoader;

        public ICommand NewCommand => _newCommand ?? new RelayCommand(() =>
                                          Navigator.ShowPage(new ItemViewControl("", new Item())));

        public IEnumerable<WorldObjectData> WorldObjects => _worldObjectLoader.WorldObjects.Values.AsEnumerable();

        public WorldObjectsManagerViewModel(IWorldObjectLoader worldObjectLoader)
        {
            _worldObjectLoader = worldObjectLoader;
        }

        public void UpdateSelectedWorldObject()
        {
            if (SelectedWorldObject == null) return;

            var dictionarySelection =
                _worldObjectLoader.WorldObjects.FirstOrDefault(m => m.Value.Name.Equals(SelectedWorldObject.Name));

            if (dictionarySelection.Key == null) return;

            OnUpdateWorldObject?.Invoke(dictionarySelection.Key, dictionarySelection.Value);
        }

    }
}