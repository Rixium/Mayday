using System.Windows.Controls;

namespace Mayday.Editor.Navigation
{
    public static class Navigator
    {
        private static ContentControl _contentControl;

        public static void InitializeWith(ContentControl contentControl) =>
            _contentControl = contentControl;


        public static void ShowPage(UserControl page) =>
            _contentControl.Content = page;
    }
}