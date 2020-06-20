using System;
using System.Windows.Input;
using Mayday.Editor.Commands;

namespace Mayday.Editor.ViewModels
{
    public class MainWindowViewModel
    {

        public Action OnCreateWorldObject { get; set; }
        public Action OnCreateItem { get; set; }

        private ICommand _createWorldObjectCommand;

        public ICommand CreateWorldObjectCommand =>
            _createWorldObjectCommand ?? (_createWorldObjectCommand = new RelayCommand(CreateWorldObject));

        private ICommand _createItemCommand;

        public ICommand CreateItemCommand =>
            _createItemCommand ?? (_createItemCommand = new RelayCommand(CreateItem));

        private void CreateItem() => OnCreateItem?.Invoke();

        private void CreateWorldObject() => OnCreateWorldObject?.Invoke();
    }
}