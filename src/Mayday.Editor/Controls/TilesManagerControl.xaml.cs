using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Controls
{
    public partial class TilesManagerControl
    {
        public TilesManagerControl()
        {
            var viewModel = new TilesManagerViewModel(
                new TileLoader()
            );

            viewModel.OnUpdateTile += (key, tile) => Navigator.ShowPage(new TileViewControl(key, tile));

            DataContext = viewModel;
            
            InitializeComponent();
        }
    }
}