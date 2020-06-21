using Mayday.Editor.Loaders;
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

            DataContext = _viewModel;
            
            InitializeComponent();
        }
    }
}