using Mayday.Editor.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Views
{
    public partial class ItemsManagerPage
    {
        public ItemsManagerPage()
        {
            var viewModel = new ItemsManagerViewModel(
                new ItemsLoader()
            );

            viewModel.OnUpdateItem += (key, item) => Navigator.ShowPage(new ItemViewControl(key, item));

            DataContext = viewModel;
            
            InitializeComponent();
        }
    }
}