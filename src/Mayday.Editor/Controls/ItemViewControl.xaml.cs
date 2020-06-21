using System.Windows.Controls;
using Mayday.Game.Gameplay.Items;

namespace Mayday.Editor.Controls
{
    public partial class ItemViewControl : UserControl
    {
        public ItemViewControl(Item item)
        {
            DataContext = item;
            InitializeComponent();
        }
    }
}