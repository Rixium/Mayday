using System.Collections.Generic;
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
using Microsoft.Xna.Framework.Media;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen
    {
        private readonly IGameRenderer _gameRenderer;
        private readonly IPlayerCreator _playerCreator;
        private readonly LightMap _lightMap = new LightMap();
        private readonly GameScreenUserInterfaceController _interfaceController;
        private readonly HashSet<IRenderable> _renderableComponents = new HashSet<IRenderable>();
        private readonly IBluePrintManager _bluePrintManager;
        private readonly Camera _camera = new Camera();
        private readonly IEntitySet _players = new EntitySet();
        private IEntity _myPlayer;
        private IEntity _currentWorldObjectPlayerIsNearTo;

        public INetworkManager NetworkManager { get; }
        public IGameWorld GameWorld { get; }

        public GameScreen(IGameWorld gameWorld, INetworkManager networkManager) : base("GameScreen")
        {
            GameWorld = gameWorld;
            NetworkManager = networkManager;

            PacketManager.Initialize(networkManager);

            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);

            _bluePrintManager = new BluePrintManager(this);

            var updateResolver = new CameraBoundsUpdateResolver(_camera);

            _gameRenderer = new GameRenderer(
                new PlayerRenderer(),
                new WorldRenderer(),
                new LightMapRenderer(),
                updateResolver);

            _playerCreator = new PlayerCreator(
                GameWorld,
                _interfaceController,
                _camera,
                updateResolver);
        }

        private void SetupTiles()
        {
            foreach (var tile in _myPlayer.GameArea.Tiles)
                _bluePrintManager.SetupFor(tile);
        }

        private void OnTilePlaced(Tile tile)
        {
            _bluePrintManager.SetupFor(tile);
            PacketManager.SendTileChangePacket(tile);
            _lightMap.Recalculate(_camera, _myPlayer.GameArea);
        }

        public IEntity AddPlayer(IEntity player, bool isClients = false)
        {
            var playerResult = isClients ?
                _playerCreator.CreateHostPlayer(player) :
                _playerCreator.CreateClientPlayer(player);

            if (isClients)
            {
                _myPlayer = player;
                _camera.SetEntity(_myPlayer, true);
            }

            if(playerResult.MouseDown != null)
                foreach (var action in playerResult.MouseDown)
                    OnMouseDown += action;

            _players.Add(player);

            return player;
        }

        public override void Awake()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(YetiGame.ContentManager.Load<Song>("gameAmbient"));
            MediaPlayer.IsRepeating = true;

            _interfaceController.ToggleMainInventory();
            _interfaceController.SetupInput();

            SetupNetworking();
            SetupWorldCallbacks();
            SetupTiles();

            _lightMap.Recalculate(_camera, _myPlayer.GameArea);
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

        private void OnTileDestroyed(Tile obj) =>
            _lightMap.Recalculate(_camera, _myPlayer.GameArea);

        private void OnNewRenderableComponentAdded(IRenderable renderableComponent) =>
            _renderableComponents.Add(renderableComponent);

        private void OnPlayerLeftRangeOfWorldObject(IEntity entity)
        {
            if (entity != _currentWorldObjectPlayerIsNearTo) return;
            _currentWorldObjectPlayerIsNearTo = null;
            _interfaceController.ClearCurrentWorldObjectForHint();
        }

        private void OnPlayerInRangeOfWorldObject(IEntity entity)
        {
            _currentWorldObjectPlayerIsNearTo = entity;
            _interfaceController.SetCurrentWorldObjectForHint(entity);
        }

        private IEntity OnClientPlayerRequested() => _myPlayer;

        private void SetupNetworking()
        {
            var gameServerListener = new MaydayServerNetworkListener(NetworkManager);
            var gameClientListener = new MaydayClientNetworkListener(NetworkManager);

            var consumers = new GamePacketConsumerManager(
                _players,
                this,
                GameWorld,
                NetworkManager.MessagePackager,
                NetworkManager);

            consumers.InjectInto(gameClientListener, gameServerListener);
        }

        public override void RenderScreen() =>
            _gameRenderer.Draw(_camera, GameWorld, _players, _myPlayer, _lightMap);

        public override void Update()
        {
            base.Update();
            NetworkManager?.Update();
            GameWorld.Update();
            _interfaceController.Update();
            _camera.Update(_myPlayer.GameArea.AreaWidth * GameWorld.TileSize, _myPlayer.GameArea.AreaHeight * GameWorld.TileSize);
        }
    }
}