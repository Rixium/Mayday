using System.Collections.Generic;
using Mayday.UI.Views;
using Myra.Graphics2D.UI;

namespace Mayday.UI.Controllers
{
    public class MainMenuUserInterfaceController
    {
        private readonly MainMenuUserInterface _userInterface;

        private Stack<Widget> _backStack = new Stack<Widget>();
        private Widget _currentActive;
        
        public MainMenuUserInterfaceController(MainMenuUserInterface userInterface)
        {
            _userInterface = userInterface;
            
            Show(_userInterface.MainPanel);
            
            _userInterface.MenuSinglePlayer.Click += (sender, args) =>
                Show(_userInterface.StartGamePanel);

            _userInterface.MenuMultiPlayer.Click += (sender, args) => 
                Show(_userInterface.MultiPlayerPanel);

            _userInterface.MenuSettings.Click += (sender, args) => 
                Show(_userInterface.SettingsPanel);
            
            _userInterface.MenuQuit.Click += (sender, args) =>
            {

            };
            
            _userInterface.StartGameBack.Click += (sender, args) => 
                ShowLast();
            
            _userInterface.MultiPlayerBack.Click += (sender, args) => 
                ShowLast();
            
            _userInterface.SettingsBack.Click += (sender, args) => 
                ShowLast();

            _userInterface.JoinByIpBack.Click += (sender, args) =>
                ShowLast();
            
            _userInterface.MultiPlayerJoinByIp.Click += (sender, args) => 
                Show(_userInterface.JoinByIpPanel);

            _userInterface.StartGameNewGame.Click += (sender, args) =>
                _userInterface.StartGamePanel.Enabled = false;
        }

        private void Show(Widget toShow)
        {
            if (toShow == null)
                return;
            
            if (_currentActive != null)
            {
                _currentActive.Visible = false;
                _backStack.Push(_currentActive);
            }

            _currentActive = toShow;
            toShow.Visible = true;
        }

        private void ShowLast()
        {
            if (_backStack.Peek() == null) return;
            
            if (_currentActive != null)
                _currentActive.Visible = false;
            
            _currentActive = _backStack.Pop();
            _currentActive.Visible = true;
        }
        
    }
}