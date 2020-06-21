using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Views
{
    public partial class ItemsManagerPage
    {

        private ItemsManagerViewModel _viewModel;

        public ItemsManagerPage()
        {
            _viewModel = new ItemsManagerViewModel(
                new ItemsLoader()
            );

            _viewModel.OnUpdateItem += (item) => Navigator.ShowPage(new ItemViewControl(item));

            DataContext = _viewModel;
            
            InitializeComponent();
        }
    }
}