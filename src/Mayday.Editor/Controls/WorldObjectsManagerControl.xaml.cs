using Mayday.Editor.Loaders;
using Mayday.Editor.Navigation;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Controls
{
    public partial class WorldObjectsManagerControl
    {
        public WorldObjectsManagerControl()
        {
            var viewModel = new WorldObjectsManagerViewModel(
                new WorldObjectLoader()
            );

            viewModel.OnUpdateWorldObject += (key, worldObject) => Navigator.ShowPage(new WorldObjectViewControl(key, worldObject));

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}