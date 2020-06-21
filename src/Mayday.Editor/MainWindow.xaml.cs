using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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

            _viewModel.OnShowItems += () => ShowPage(new ItemsManagerPage());

            DataContext = _viewModel;

            InitializeComponent();
        }

        private void ShowPage(UserControl page)
        {

            MainContent.Content = page;
        }
    }
}