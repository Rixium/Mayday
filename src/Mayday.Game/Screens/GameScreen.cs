using System.Collections.Generic;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Blueprints;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Creators;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Lighting;
using Mayday.Game.Networking;
using Mayday.Game.Networking.Consumers;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Optimization;
using Mayday.Game.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen
    {
        public INetworkManager NetworkManager { get; }
        public IGameWorld GameWorld => _gameWorld;
        public IBluePrintManager BluePrintManager { get; set; }

        public readonly IPlayerSet Players = new PlayerSet();
        public IEntity MyPlayer { get; private set; }

        public IEntity CurrentWorldObjectPlayerIsNearTo { get; set; }

        public Camera Camera { get; } = new Camera();

        private readonly IGameWorld _gameWorld;
        private readonly IGameRenderer _gameRenderer;
        private IPlayerCreator _playerCreator;
        private readonly LightMap _lightMap = new LightMap();
        private readonly GameScreenUserInterfaceController _interfaceController;
        private readonly HashSet<IRenderable> _renderableComponents = new HashSet<IRenderable>();
        private CameraBoundsUpdateResolver _updateResolver;

        public GameScreen(IGameWorld gameWorld, INetworkManager networkManager) : base("GameScreen")
        {
            _gameWorld = gameWorld;
            NetworkManager = networkManager;

            PacketManager.Initialize(networkManager);

            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);

            BluePrintManager = new BluePrintManager(this);

            _updateResolver = new CameraBoundsUpdateResolver(Camera);

            _gameRenderer = new GameRenderer(
                new PlayerRenderer(),
                new WorldRenderer(),
                new LightMapRenderer(),
                _updateResolver);

            _playerCreator = new PlayerCreator(
                GameWorld,
                _interfaceController,
                Camera,
                _updateResolver);
        }

        private void SetupTiles()
        {
            foreach (var tile in GameWorld.GameAreas[0].Tiles)
                SetupTile(tile);
        }

        private void OnTilePlaced(Tile tile)
        {
            SetupTile(tile);
            PacketManager.SendTileChangePacket(tile);

            RecalculateLighting();
        }

        private async void RecalculateLighting() =>
            await Task.Run(async () =>
            {
                await _lightMap.CheckLights(MyPlayer.GameArea);
            });

        private void SetupTile(IEntity tile) => BluePrintManager.SetupFor(tile);

        public IEntity AddPlayer(IEntity player, bool isClients = false)
        {
            player = isClients ?
                _playerCreator.CreateHostPlayer(player) :
                _playerCreator.CreateClientPlayer(player);

            if (isClients)
                MyPlayer = player;

            Players.Add(player);

            return player;
        }

        public override void Awake()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(YetiGame.ContentManager.Load<Song>("gameAmbient"));
            MediaPlayer.IsRepeating = true;

            _interfaceController.ToggleMainInventory();

            BackgroundColor = new Color(47, 39, 54);

            SetupNetworking();
            SetupWorldCallbacks();
            SetupTiles();
            SetupUiInput();

            Camera.SetEntity(MyPlayer);

            var cameraPosition = Camera.Position;
            cameraPosition.X = MyPlayer.Center.X;
            cameraPosition.Y = MyPlayer.Y - Window.ViewportHeight / 2.0f;
            Camera.Position = cameraPosition;

            RecalculateLighting();
        }

        private void SetupUiInput()
        {
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D1),
                () => _interfaceController.InventorySelectionChanged(0));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D2),
                () => _interfaceController.InventorySelectionChanged(1));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D3),
                () => _interfaceController.InventorySelectionChanged(2));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D4),
                () => _interfaceController.InventorySelectionChanged(3));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D5),
                () => _interfaceController.InventorySelectionChanged(4));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D6),
                () => _interfaceController.InventorySelectionChanged(5));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D7),
                () => _interfaceController.InventorySelectionChanged(6));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D8),
                () => _interfaceController.InventorySelectionChanged(7));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.I),
                () => _interfaceController.ToggleMainInventory());
        }

        private void SetupWorldCallbacks()
        {
            GameWorld.TilePlaced += OnTilePlaced;
            GameWorld.TileDestroyed += OnTileDestroyed;
            GameWorld.RequestClientPlayer += OnClientPlayerRequested;
            GameWorld.PlayerInRangeOfWorldObject += OnPlayerInRangeOfWorldObject;
            GameWorld.PlayerLeftRangeOfWorldObject += OnPlayerLeftRangeOfWorldObject;
            GameWorld.RenderableComponentAdded += OnNewRenderableComponentAdded;
        }

        private void OnTileDestroyed(Tile obj) => RecalculateLighting();

        private void OnNewRenderableComponentAdded(IRenderable renderableComponent) =>
            _renderableComponents.Add(renderableComponent);

        private void OnPlayerLeftRangeOfWorldObject(IEntity entity)
        {
            if (entity != CurrentWorldObjectPlayerIsNearTo) return;
            CurrentWorldObjectPlayerIsNearTo = null;
            _interfaceController.ClearCurrentWorldObjectForHint();
        }

        private void OnPlayerInRangeOfWorldObject(IEntity entity)
        {
            CurrentWorldObjectPlayerIsNearTo = entity;
            _interfaceController.SetCurrentWorldObjectForHint(entity);
        }

        private IEntity OnClientPlayerRequested() => MyPlayer;

        private void SetupNetworking()
        {
            var gameServerListener = new MaydayServerNetworkListener(NetworkManager);
            var gameClientListener = new MaydayClientNetworkListener(NetworkManager);
            var consumers = new GamePacketConsumerManager(this);
            consumers.InjectInto(gameClientListener, gameServerListener);
        }

        public override void RenderScreen() =>
            _gameRenderer.Draw(Camera, GameWorld, Players, MyPlayer, _lightMap);

        public override void Update()
        {
            base.Update();

            NetworkManager?.Update();

            GameWorld.Update();

            if (MouseState.CurrentState.ScrollWheelValue > MouseState.LastState.ScrollWheelValue)
                _interfaceController.IncrementSelection(-1);
            else if (MouseState.CurrentState.ScrollWheelValue < MouseState.LastState.ScrollWheelValue)
                _interfaceController.IncrementSelection(1);

            Camera.Update(MyPlayer.GameArea.AreaWidth * GameWorld.TileSize, MyPlayer.GameArea.AreaHeight * GameWorld.TileSize);
        }
    }
}