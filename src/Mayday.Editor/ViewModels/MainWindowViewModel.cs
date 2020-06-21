using System;
using System.Windows.Input;
using Mayday.Editor.Commands;
using Mayday.Editor.Navigation;

namespace Mayday.Editor.ViewModels
{
    public class MainWindowViewModel
    {
        public Action OnShowWorldObjects { get; set; }
        public Action OnShowItems { get; set; }

        private ICommand _itemCommand;

        public ICommand ItemCommand =>
            _itemCommand ?? (_itemCommand = new RelayCommand(ShowItems));

        private ICommand _worldObjectCommand;

        public ICommand WorldObjectCommand =>
            _worldObjectCommand ?? (_worldObjectCommand = new RelayCommand(ShowWorldObjects));

        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ?? (_backCommand = new RelayCommand(Navigator.Back));

        private void ShowWorldObjects() => OnShowWorldObjects?.Invoke();

        private void ShowItems() => OnShowItems?.Invoke();
    }
}