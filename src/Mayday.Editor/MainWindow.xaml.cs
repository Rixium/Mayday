using System;
using System.Windows;
using Mayday.Editor.ViewModels;
using Mayday.Editor.Views;

namespace Mayday.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();

            _viewModel.OnCreateItem += () => ShowWindow(new NewItemWindow());
            _viewModel.OnCreateWorldObject += () => ShowWindow(new NewWorldObjectWindow());

            DataContext = _viewModel;

            InitializeComponent();
        }

        private void ShowWindow(Window window) => window.Show();
    }
}