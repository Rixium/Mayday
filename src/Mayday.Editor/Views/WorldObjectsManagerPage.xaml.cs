using System.Windows.Controls;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Views
{
    public partial class WorldObjectsManagerPage
    {
        public WorldObjectsManagerPage()
        {
            DataContext = new WorldObjectsManagerViewModel();

            InitializeComponent();
        }
    }
}