using System.Windows.Controls;
using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;
using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.Controls
{
    public partial class TileViewControl
    {
        public TileViewControl(string key, TileProperties tile)
        {
            DataContext = new TileViewViewModel(key, tile, new TileLoader(), new ItemsLoader());
            InitializeComponent();
        }

    }
}