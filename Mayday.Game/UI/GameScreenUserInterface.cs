using GeonBit.UI;
using GeonBit.UI.Entities;
using GeonBit.UI.Utils;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using Yetiface.Engine.Networking;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;

namespace Mayday.Game.UI
{
    public class GameScreenUserInterface : IUserInterface
    {
        private readonly GameScreen _gameScreen;
        private readonly INetworkManager _netManager;
        private MaydayMessagePackager _packager;
        
        private KeyboardState _lastState;

        private MessageBox.MessageBoxHandle _chatBox;
        public readonly Entity TextInput;
        
        public GameScreenUserInterface(GameScreen gameScreen, INetworkManager netManager)
        {
            _gameScreen = gameScreen;
            _netManager = netManager;
            UserInterface.Active.Clear();
            
            _packager = new MaydayMessagePackager();

            var mainPanel = UserInterface.Active.AddEntity(new Panel(new Vector2(0, 0), PanelSkin.None));
            TextInput = mainPanel.AddChild(new TextInput(false, new Vector2(0.9f, 70), Anchor.BottomCenter,
                    new Vector2(0, -10), PanelSkin.Simple));
        }
        
        public void Draw()
        {
            UserInterface.Active.Draw(GraphicsUtils.Instance.SpriteBatch);
        }

        public void Update()
        {
            UserInterface.Active.Update(Time.GameTime);

            var keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyDown(Keys.Enter) && _lastState.IsKeyUp(Keys.Enter))
            {
                ShowChatBox();
            }

            _lastState = keyboardState;
        }

        private void ShowChatBox()
        {
            if (TextInput.IsFocused)
            {
                SendMessage();
                return;
            }

            TextInput.IsFocused = true;
            
            UserInterface.Active.ActiveEntity = TextInput;
        }

        private void SendMessage()
        {
            TextInput.IsFocused = false;
            
            if (((TextInput) TextInput).Value == "") return;
            
            var chatMessage = new ChatMessagePacket
            {
                SteamId = SteamClient.SteamId,
                Message = ((TextInput)TextInput).Value
            };

            ((TextInput) TextInput).Value = "";
            
            var package = _packager.Package(chatMessage);
            
            _netManager.SendMessage(package);
            _gameScreen.AddChat(chatMessage);
        }

        public void AfterDraw()
        {
            UserInterface.Active.DrawMainRenderTarget(GraphicsUtils.Instance.SpriteBatch);
        }

    }
}