using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Controls
{
    public partial class WorldObjectsManagerControl
    {
        private WorldObjectsManagerViewModel _viewModel;

        public WorldObjectsManagerControl()
        {
            _viewModel = new WorldObjectsManagerViewModel(
                new WorldObjectLoader()
            );

            _viewModel.OnUpdateWorldObject += (key, worldObject) => Navigator.ShowPage(new WorldObjectViewControl(key, worldObject));

            DataContext = _viewModel;

            InitializeComponent();
        }
    }
}