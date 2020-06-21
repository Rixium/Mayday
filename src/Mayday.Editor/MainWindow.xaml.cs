using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Mayday.Editor.Controls;
using Mayday.Editor.Navigation;
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

            _viewModel.OnShowItems += () => Navigator.ShowPage(new ItemsManagerPage());
            _viewModel.OnShowWorldObjects += () => Navigator.ShowPage(new WorldObjectsManagerControl());
            _viewModel.OnShowTiles += () => Navigator.ShowPage(new TilesManagerControl());

            DataContext = _viewModel;

            InitializeComponent();

            Navigator.InitializeWith(MainContent);
        }

    }
}