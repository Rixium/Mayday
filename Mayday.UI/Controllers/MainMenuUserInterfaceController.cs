using System.Collections.Generic;
using Mayday.UI.Views;
using Myra.Graphics2D.UI;

namespace Mayday.UI.Controllers
{
    public class MainMenuUserInterfaceController
    {
        private MainMenuUserInterface UserInterface { get; set; }

        private Stack<Widget> _backStack = new Stack<Widget>();
        private Widget _currentActive;
        
        public MainMenuUserInterfaceController(MainMenuUserInterface userInterface)
        {
            UserInterface = userInterface;
            
            Show(UserInterface.MainPanel);
            
            UserInterface.MenuSinglePlayer.Click += (sender, args) =>
                Show(UserInterface.StartGamePanel);

            UserInterface.MenuMultiPlayer.Click += (sender, args) => 
                Show(UserInterface.MultiPlayerPanel);

            UserInterface.MenuSettings.Click += (sender, args) => 
                Show(UserInterface.SettingsPanel);
            
            UserInterface.MenuQuit.Click += (sender, args) =>
            {

            };
            
            UserInterface.StartGameBack.Click += (sender, args) => 
                ShowLast();
            
            UserInterface.MultiPlayerBack.Click += (sender, args) => 
                ShowLast();
            
            UserInterface.SettingsBack.Click += (sender, args) => 
                ShowLast();

            UserInterface.JoinByIpBack.Click += (sender, args) =>
                ShowLast();
            
            UserInterface.MultiPlayerJoinByIp.Click += (sender, args) => 
                Show(UserInterface.JoinByIpPanel);

            UserInterface.StartGameNewGame.Click += (sender, args) =>
                UserInterface.StartGamePanel.Enabled = false;
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