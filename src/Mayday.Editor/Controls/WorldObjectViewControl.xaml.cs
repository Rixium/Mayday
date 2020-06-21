using Mayday.Editor.Loaders;
using Mayday.Editor.ViewModels;
using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.Controls
{
    public partial class WorldObjectViewControl
    {
        public WorldObjectViewControl(string key, WorldObjectData worldObjectData)
        {
            DataContext = new WorldObjectViewViewModel(key, worldObjectData, new WorldObjectLoader());
            InitializeComponent();
        }

    }
}