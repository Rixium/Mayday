using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.Controls
{
    public partial class ItemViewControl
    {
        public ItemViewControl(Item item)
        {
            DataContext = new ItemViewViewModel(item,
                new WorldObjectLoader(),
                new TileLoader());

            InitializeComponent();
        }
    }
}