using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        private Image _imageElement;
        private Panel _mainPanel;
        private Panel _singlePlayerPanel;
        private Panel _multiplayerPanel;
        private Panel _settingsPanel;

        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;
        }

        public override void Awake()
        {
            var image = YetiGame.ContentManager.Load<Texture2D>("MainMenu/planet");

            _imageElement = UserInterface.AddElement(
                new Image(image)
                {
                    Size = new Vector2(0.5f, 0.5f),
                    Anchor = Anchor.Center
                }
            );

            var titlePanel = UserInterface.AddElement(new Panel()
            {
                FillColor = Color.Black * 0.5f,
                Anchor = Anchor.TopCenter,
                Size = new Vector2(1, 0.2f)
            });
            
            titlePanel.AddElement(new TextBlock("Mayday"));

            SetupMainMenuPanel();
            SetupSinglePlayerPanel();
            SetupMultiplayerPanel();
            SetupSettingsPanel();
        }

        private void SetupMainMenuPanel()
        {
            _mainPanel = UserInterface.AddElement(new Panel
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(300, 0)
            });

            var singlePlayerButton = _mainPanel.AddElement(new Button("Single Player")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -220),
                FillColor = new Color(210, 125, 44)
            });
            
            var multiplayerButton = _mainPanel.AddElement(new Button("Multiplayer")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -150),
                FillColor = new Color(210, 125, 44)
            });
            
            var settingsButton = _mainPanel.AddElement(new Button("Settings")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -80),
                FillColor = new Color(210, 125, 44)
            });
            
            var exitButton = _mainPanel.AddElement(new Button("Exit")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -10),
                FillColor = new Color(210, 125, 44)
            });

            singlePlayerButton.OnHover += OnButtonHover;
            singlePlayerButton.OnLeave += OnHoverLeave;
            multiplayerButton.OnHover += OnButtonHover;
            multiplayerButton.OnLeave += OnHoverLeave;
            settingsButton.OnHover += OnButtonHover;
            settingsButton.OnLeave += OnHoverLeave;
            exitButton.OnHover += OnButtonHover;
            exitButton.OnLeave += OnHoverLeave;
            exitButton.OnClicked += (element) => YetiGame.Quit();
            
            singlePlayerButton.OnClicked += (element) =>
            {
                _mainPanel.Active = false;
                _singlePlayerPanel.Active = true;
            };
            
            multiplayerButton.OnClicked += (element) =>
            {
                _mainPanel.Active = false;
                _multiplayerPanel.Active = true;
            };
            
            settingsButton.OnClicked += (element) =>
            {
                _mainPanel.Active = false;
                _settingsPanel.Active = true;
            };
        }

        private void SetupSinglePlayerPanel()
        {
            // Second panel is for singleplayer.
            _singlePlayerPanel = UserInterface.AddElement(new Panel
            {
                Active = false,
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(300, 0)
            });

            var newGameButton = _singlePlayerPanel.AddElement(new Button("New Game")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -150),
                FillColor = new Color(210, 125, 44)
            });
            
            var loadGameButton = _singlePlayerPanel.AddElement(new Button("Load Game")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -80),
                FillColor = new Color(210, 125, 44)
            });
            
            var backButton = _singlePlayerPanel.AddElement(new Button("Back")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -10),
                FillColor = new Color(210, 125, 44)
            });

            newGameButton.OnHover += OnButtonHover;
            newGameButton.OnLeave += OnHoverLeave;
            loadGameButton.OnHover += OnButtonHover;
            loadGameButton.OnLeave += OnHoverLeave;
            backButton.OnHover += OnButtonHover;
            backButton.OnLeave += OnHoverLeave;
            backButton.OnClicked += (element) =>
            {
                _singlePlayerPanel.Active = false;
                _mainPanel.Active = true;
            };
        }

        private void SetupMultiplayerPanel()
        {
            _multiplayerPanel = UserInterface.AddElement(new Panel
            {
                Active = false,
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(300, 0)
            });

            var startServer = _multiplayerPanel.AddElement(new Button("Start Server")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -150),
                FillColor = new Color(210, 125, 44)
            });
            
            var joinGame = _multiplayerPanel.AddElement(new Button("Join Game")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -80),
                FillColor = new Color(210, 125, 44)
            });
            
            var backButton = _multiplayerPanel.AddElement(new Button("Back")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -10),
                FillColor = new Color(210, 125, 44)
            });

            startServer.OnHover += OnButtonHover;
            startServer.OnLeave += OnHoverLeave;
            joinGame.OnHover += OnButtonHover;
            joinGame.OnLeave += OnHoverLeave;
            backButton.OnHover += OnButtonHover;
            backButton.OnLeave += OnHoverLeave;
            backButton.OnClicked += (element) =>
            {
                _multiplayerPanel.Active = false;
                _mainPanel.Active = true;
            };
        }
        
        private void SetupSettingsPanel()
        {
            _settingsPanel = UserInterface.AddElement(new Panel
            {
                Active = false,
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(300, 0)
            });

            var resizeButton = _settingsPanel.AddElement(new Button("Resize")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -80),
                FillColor = new Color(210, 125, 44)
            });
            
            var backButton = _settingsPanel.AddElement(new Button("Back")
            {
                Anchor = Anchor.BottomLeft,
                Size = new Vector2(1, 60),
                Offset = new Vector2(10, -10),
                FillColor = new Color(210, 125, 44)
            });

            resizeButton.OnHover += OnButtonHover;
            resizeButton.OnLeave += OnHoverLeave;
            backButton.OnHover += OnButtonHover;
            backButton.OnLeave += OnHoverLeave;
            backButton.OnClicked += (element) =>
            {
                _settingsPanel.Active = false;
                _mainPanel.Active = true;
            };

            resizeButton.OnClicked += (element) => Game1.NextResize();
        }

        private void OnHoverLeave(IElement obj)
        {
            obj.FillColor = new Color(210, 125, 44);
        }

        private void OnButtonHover(IElement obj)
        {
            obj.FillColor = new Color(180, 95, 14);
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }

        public override void Update()
        {
            base.Update();

            _imageElement.Rotation += Time.DeltaTime * 0.05f;
        }
    }
}