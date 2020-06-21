using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Controls
{
    public partial class TilesManagerControl
    {

        private TilesManagerViewModel _viewModel;

        public TilesManagerControl()
        {
            _viewModel = new TilesManagerViewModel(
                new TileLoader()
            );

            // _viewModel.OnUpdateTile += (key, tile) => Navigator.ShowPage(new TileViewControl(key, tile));

            DataContext = _viewModel;
            
            InitializeComponent();
        }
    }
}