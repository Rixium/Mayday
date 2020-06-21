using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Editor.Popups;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.ViewModels
{
    public class WorldObjectViewViewModel
    {
        private readonly IWorldObjectLoader _worldObjectLoader;

        private ICommand _saveItemCommand;
        public ICommand SaveItemCommand => _saveItemCommand ?? new RelayCommand(SaveItem);

        public string Key { get; }
        public WorldObjectData WorldObjectData { get; }

        public string WorldObjectId
        {
            get => WorldObjectData.Id;
            set
            {
                if (WorldObjectData.Id != null && WorldObjectData.Id.Equals(value))
                    return;

                WorldObjectData.Id = value;
                OnPropertyChanged();
            }
        }

        public string WorldObjectName
        {
            get => WorldObjectData.Name;
            set
            {
                if (WorldObjectData.Name.Equals(value))
                    return;

                WorldObjectData.Name = value;
                OnPropertyChanged();
            }
        }

        public WorldObjectViewViewModel(string key, WorldObjectData worldObjectData,
            IWorldObjectLoader worldObjectLoader)
        {
            _worldObjectLoader = worldObjectLoader;

            Key = key;
            WorldObjectData = worldObjectData;
        }

        private void SaveItem()
        {
            var worldObjects = _worldObjectLoader.WorldObjects;

            if (worldObjects.ContainsKey(Key))
            {
                if (Key.Equals(WorldObjectData.Id))
                {
                    worldObjects[Key] = WorldObjectData;
                }
                else
                {
                    worldObjects.Remove(Key);
                    worldObjects.Add(WorldObjectData.Id, WorldObjectData);
                }
            }
            else
            {
                worldObjects.Add(WorldObjectData.Id, WorldObjectData);
            }

            _worldObjectLoader.SetWorldObjects(worldObjects);
            _worldObjectLoader.Save();

            CheckTextureExists();
            
            Navigator.ShowPage(new WorldObjectsManagerControl());
        }

        private void CheckTextureExists()
        {
            var _filePath = $"..\\..\\..\\src\\Mayday.Game\\Content\\Images\\WorldObjects\\{WorldObjectData.Id}.png";
            if (File.Exists(_filePath))
            {
                return;
            }

            var window = new WarningPopup
            {
                WarningText =
                {
                    Text = $"{WorldObjectData.Id}.png does not exist in the images\\worldobjects folder."
                }
            };

            window.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}