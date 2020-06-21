using System.Collections.Generic;
using System.Windows.Controls;

namespace Mayday.Editor.Navigation
{
    public static class Navigator
    {

        private static Stack<UserControl> _contentStack = new Stack<UserControl>();

        private static ContentControl _contentControl;

        private static UserControl _currentPage;

        public static void InitializeWith(ContentControl contentControl) =>
            _contentControl = contentControl;


        public static void ShowPage(UserControl page)
        {
            if(_currentPage != null)
                _contentStack.Push(_currentPage);

            _currentPage = page;
            _contentControl.Content = _currentPage;
        }

        public static void Back()
        {
            if (_contentStack.Count == 0) return;
            var lastControl = _contentStack.Pop();
            ShowPage(lastControl);
        }
    }
}