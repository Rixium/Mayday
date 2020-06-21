using System.Windows;
using System.Windows.Controls;
using Mayday.Editor.ViewModels;

namespace Mayday.Editor.Views
{
    public partial class ItemsManagerPage
    {
        public ItemsManagerPage()
        {
            DataContext = new ItemsManagerViewModel();
            
            InitializeComponent();
        }
    }
}