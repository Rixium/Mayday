using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.Controls
{
    public partial class ItemViewControl
    {
        public ItemViewControl(string key, Item item)
        {
            DataContext = new ItemViewViewModel(
                key,
                item,
                new WorldObjectLoader(),
                new TileLoader(),
                new ItemsLoader());

            InitializeComponent();
        }
    }
}