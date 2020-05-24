using GeonBit.UI;
using GeonBit.UI.Animators;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.Utils;

namespace Mayday.Game.UI
{
    public class MenuScreenUserInterface : IUserInterface
    {
        private readonly UserInterface _active;
        
        private Entity _rootPanel;
        private Entity _mainMenuButtonPanel;
        private Entity _singlePlayerPanel;
        private Entity _multiplayerPanel;
        private Entity _hostGamePanel;
        private Entity _joinGamePanel;
        private Entity _settingsPanel;

        public MenuScreenUserInterface()
        {
            UserInterface.Initialize(YetiGame.ContentManager);
            _active = UserInterface.Active;
            
            _rootPanel =  UserInterface.Active.AddEntity(new Panel(new Vector2(0, 0))
            {
                FillColor = Color.Black
            });

            _rootPanel.Padding = new Vector2(30, 30);

            var centerPanel = _rootPanel.AddChild(new Panel(new Vector2(0.5f, 0.5f), PanelSkin.None));
            
            var image = centerPanel.AddChild(new Image(YetiGame.ContentManager.Load<Texture2D>("MainMenu/planet"), new Vector2(0.8f, 0.8f), ImageDrawMode.Stretch, Anchor.Center)
            {
                EnforceSquare = true
            });
            
            image.SpaceBefore = new Vector2(100, 100);
            image.SpaceAfter =  new Vector2(100, 100);


            image.AttachAnimator(new RotationAnimator());
            
            var titlePanel = _rootPanel.AddChild(new Panel(new Vector2(0, 0.1f), PanelSkin.None, Anchor.TopCenter));
            var title = titlePanel.AddChild(new Header("Mayday", Anchor.Center)
            {
                FillColor = Color.White,
                OutlineColor = Color.Black
            });

            SetupMainMenuPanel();
            SetupSinglePlayerPanel();
            SetupMultiplayerPanel();
            SetupHostGamePanel();
            SetupJoinGamePanel();
            SetupSettingsPanel();
            
            SetupFooter();

        }

        private void SetupMainMenuPanel()
        {
            _mainMenuButtonPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center));
            var singlePlayerButton = _mainMenuButtonPanel.AddChild(new Button("Single Player"));
            var multiplayerButton = _mainMenuButtonPanel.AddChild(new Button("Multiplayer"));
            var settingsButton = _mainMenuButtonPanel.AddChild(new Button("Settings"));
            var exitButton = _mainMenuButtonPanel.AddChild(new Button("Exit"));
            
            singlePlayerButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = false;
                _singlePlayerPanel.Visible = true;
            };
            
            multiplayerButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = false;
                _multiplayerPanel.Visible = true;
            };
            
            settingsButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = false;
                _settingsPanel.Visible = true;
            };
            
            exitButton.OnClick += (e) =>
            {
                YetiGame.Quit();
            };
        }

        private void SetupSinglePlayerPanel()
        {
            _singlePlayerPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center)
            {
                Visible = false
            });
            
            _singlePlayerPanel.AddChild(new Button("New Game"));
            _singlePlayerPanel.AddChild(new Button("Load Game"));
            var backButton = _singlePlayerPanel.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _singlePlayerPanel.Visible = false;
            };
        }
        
        private void SetupMultiplayerPanel()
        {
            _multiplayerPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center)
            {
                Visible = false
            });
            
            var hostGameButton = _multiplayerPanel.AddChild(new Button("Host Game"));
            var joinGameButton = _multiplayerPanel.AddChild(new Button("Join Game"));
            var backButton = _multiplayerPanel.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _multiplayerPanel.Visible = false;
            };
                        
            hostGameButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = false;
                _hostGamePanel.Visible = true;
            };      
            
            joinGameButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = false;
                _joinGamePanel.Visible = true;
            };
        }
     
        private void SetupHostGamePanel()
        {
            _hostGamePanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center)
            {
                Visible = false
            });
            
            _hostGamePanel.AddChild(new Button("New Game"));
            _hostGamePanel.AddChild(new Button("Load Game"));
            var backButton = _hostGamePanel.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = true;
                _hostGamePanel.Visible = false;
            };
        }     
        
        private void SetupJoinGamePanel()
        {
            _joinGamePanel = _rootPanel.AddChild(new Panel(new Vector2(0, 0), PanelSkin.None, Anchor.Center)
            {
                Visible = false
            });

            _joinGamePanel.AddChild(new SelectList(new Vector2(0.2f, 0.2f), Anchor.CenterLeft));
            
            var joinGamePanelButtons =
                _joinGamePanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center));
            
            joinGamePanelButtons.AddChild(new Button("Join By IP"));
            var backButton = joinGamePanelButtons.AddChild(new Button("Back"));
            
            backButton.OnClick += (e) =>
            {
                _multiplayerPanel.Visible = true;
                _joinGamePanel.Visible = false;
            };
        }        
        
        private void SetupSettingsPanel()
        {
            _settingsPanel = _rootPanel.AddChild(new Panel(new Vector2(400, -1), PanelSkin.None, Anchor.Center)
            {
                Visible = false
            });
            
            var resolutionButton = _settingsPanel.AddChild(new Button("Change Resolution"));
            var backButton = _settingsPanel.AddChild(new Button("Back"));
            
            resolutionButton.OnClick += (e) => Game1.NextResize();
            
            backButton.OnClick += (e) =>
            {
                _mainMenuButtonPanel.Visible = true;
                _settingsPanel.Visible = false;
            };
        }

        private void SetupFooter()
        {
            var bottomText = _rootPanel.AddChild(new RichParagraph("YetiFace", Anchor.BottomCenter)
            {
                Scale = 0.8f
            });
            
            bottomText.AttachAnimator(new TextWaveAnimator());
        }

        public void Draw() => _active.Draw(GraphicsUtils.Instance.SpriteBatch);

        public void Update() => _active.Update(Time.GameTime);
    }
}