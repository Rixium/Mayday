using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Views
{
    public partial class ItemsManagerPage
    {
        public ItemsManagerPage()
        {
            DataContext = new ItemsManagerViewModel(
                new ItemsLoader()
            );
            
            InitializeComponent();
        }
    }
}