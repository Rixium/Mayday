using System.Windows.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Controls
{
    public partial class WorldObjectsManagerControl : UserControl
    {
        private WorldObjectsManagerViewModel _viewModel;

        public WorldObjectsManagerControl()
        {
            _viewModel = new WorldObjectsManagerViewModel(
                new WorldObjectLoader()
            );

            // _viewModel.OnUpdateTile += (key, tile) => Navigator.ShowPage(new TileViewControl(key, tile));

            DataContext = _viewModel;

            InitializeComponent();
        }
    }
}