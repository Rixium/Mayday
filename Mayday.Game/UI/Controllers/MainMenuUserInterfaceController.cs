using System.Collections.Generic;
using Mayday.UI.Views;
using Myra.Graphics2D.UI;

namespace Mayday.Game.UI.Controllers
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

            UserInterface.MultiPlayerCreateGame.Click += (sender, args) =>
                Show(UserInterface.CreateMultiplayerGamePanel);

            UserInterface.MenuSettings.Click += (sender, args) =>
                Show(UserInterface.SettingsPanel);

            UserInterface.MenuQuit.Click += (sender, args) => { };

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

            UserInterface.CreateMultiplayerGameCreateGame.Click += (sender, args) =>
                UserInterface.CreateMultiplayerGamePanel.Visible = false;

            UserInterface.StartGameNewGame.Click += (sender, args) =>
                UserInterface.StartGamePanel.Visible = false;

            UserInterface.JoinByIpJoin.Click += (sender, args) =>
                UserInterface.JoinByIpPanel.Visible = false;
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
            if (_backStack.Count == 0) return;

            if (_currentActive != null)
                _currentActive.Visible = false;

            _currentActive = _backStack.Pop();
            _currentActive.Visible = true;
        }
    }
}