using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Blueprints;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.Tutorials;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
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
using Yetiface.Engine.Optimization;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen
    {
        public INetworkManager NetworkManager { get; }
        public IGameWorld GameWorld { get; private set; }
        public IUpdateResolver<IEntity> UpdateResolver;
        public IBluePrintManager BluePrintManager { get; set; }

        public readonly IPlayerSet Players = new PlayerSet();
        public IEntity MyPlayer { get; private set; }

        private readonly IWorldRenderer _worldRenderer;
        private readonly IPlayerRenderer _playerRenderer;
        private readonly LightmapRenderer _lightmapRenderer = new LightmapRenderer();
        private readonly Lightmap _lightmap = new Lightmap();
        private readonly GameScreenUserInterfaceController _interfaceController;
        private readonly HashSet<IRenderable> _renderableComponents = new HashSet<IRenderable>();
        private float[,] _lights;

        public IEntity CurrentWorldObjectPlayerIsNearTo { get; set; }

        public Camera Camera { get; } = new Camera();

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            NetworkManager = networkManager;
            PacketManager.Initialize(networkManager);

            _worldRenderer = new WorldRenderer();
            _playerRenderer = new PlayerRenderer();

            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);

            BluePrintManager = new BluePrintManager(this);
            UpdateResolver = new CameraBoundsUpdateResolver(Camera);
        }

        public void SetWorld(IGameWorld gameWorld) => GameWorld = gameWorld;

        private void SetupTiles()
        {
            foreach (var tile in GameWorld.GameAreas[0].Tiles)
                SetupTile(tile);
        }

        private void OnTilePlaced(Tile tile)
        {
            SetupTile(tile);
            _lights = _lightmap.CheckLights(MyPlayer, Camera);
            PacketManager.SendTileChangePacket(tile);
        }

        private void SetupTile(IEntity tile) => BluePrintManager.SetupFor(tile);

        public IEntity AddPlayer(IEntity player, bool isClients = false)
        {
            if (isClients)
            {
                var spawnTile = GameWorld.GameAreas[0].GetRandomSpawnLocation();
                player.X = spawnTile.TileX * GameWorld.TileSize;
                player.Y = spawnTile.TileY * GameWorld.TileSize - (70 * 4);
                MyPlayer = player;
            }

            player.GameWorld = GameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[1].Animations)
            };

            var moveComponent = player.AddComponent(new MoveComponent());
            var gravityComponent = player.AddComponent(new GravityComponent());
            var jumpComponent = player.AddComponent(new JumpComponent());
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));
            var itemPickerComponent =
                player.AddComponent(new ItemPickerComponent(GameWorld.GameAreas[0].WorldItems, UpdateResolver));
            playerAnimationComponent = player.AddComponent(playerAnimationComponent);

            moveComponent.PositionChanged += PacketManager.SendPositionPacket;
            moveComponent.MoveDirectionChanged += PacketManager.SendMoveDirectionPacket;

            player.Bounds = new RectangleF(9, 10, 13, 30);

            if (isClients)
            {
                var blockBreakerComponent = player.AddComponent(new BlockBreakerComponent(GameWorld, Camera));
                player.AddComponent(new CharacterControllerComponent());
                var itemPlacerComponent = player.AddComponent(new ItemPlacerComponent(this));
                _interfaceController.SelectedItemSlotChanged += (i) =>
                {
                    var item = inventoryBar.GetItemAt(i);
                    itemPlacerComponent.SetSelectedItem(item);
                };
                itemPlacerComponent.ItemUsed += (item) => inventoryBar.RemoveItem(item);

                OnMouseDown += blockBreakerComponent.MouseDown;
                OnMouseDown += itemPlacerComponent.MouseDown;
                inventoryBar.InventoryChanged += () => _interfaceController.InventoryBarChanged(inventoryBar);
                mainInventory.InventoryChanged += () => _interfaceController.MainInventoryChanged(mainInventory);
                jumpComponent.Jump += PacketManager.SendJumpPacket;
                inventoryBar.AddItemToInventory(ContentChest.ItemData["Shuttle"]);

                var tutorialManagerComponent = player.AddComponent(new TutorialManagerComponent());

                var tutorial1 = new PopupTutorial<IEntity>(new TutorialDefinition()
                {
                    Text = "HELLO WORLD!",
                });


                tutorialManagerComponent.AddTutorial("Test", tutorial1);

                GameWorld.PlayerInRangeOfWorldObject += tutorial1.Trigger;
            }

            // TODO LOTS
            player.GameArea = GameWorld.GameAreas[0];

            Players.Add(player);

            GameWorld.AddTrackedEntity(player);

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

            _lights = _lightmap.CheckLights(MyPlayer, Camera);
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

        private void OnTileDestroyed(Tile obj) => _lights = _lightmap.CheckLights(MyPlayer, Camera);

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

        public override void RenderScreen()
        {
            _lightmapRenderer.RenderToRenderTarget(_lights, GameWorld, MyPlayer);

            GraphicsUtils.Instance.Begin();
            GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.Background, new Rectangle(0, 0, Window.ViewportWidth, Window.ViewportHeight), Color.White);
            GraphicsUtils.Instance.End();
            
            GraphicsUtils.Instance.Begin(true, Camera.GetMatrix());

            _worldRenderer.DrawWorldObjects(GameWorld.GameAreas[0], Camera);
            _playerRenderer.DrawPlayers(Players.GetAll());

            foreach (var entity in GameWorld.GameAreas[0].GetItems())
            {
                if (!UpdateResolver.ShouldUpdate(entity)) continue;
                if (!(entity is ItemDrop item)) continue;
                GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.ItemTextures[item.Item.ItemId],
                    new Vector2(item.X, item.Y), Color.White);
            }

            _worldRenderer.Draw(GameWorld.GameAreas[0], Camera);

            GraphicsUtils.Instance.End();

            _lightmapRenderer.Draw(GameWorld, MyPlayer, Camera);
        }

        public override void Update()
        {
            base.Update();

            NetworkManager?.Update();

            GameWorld.Update();

            if (MouseState.CurrentState.ScrollWheelValue > MouseState.LastState.ScrollWheelValue)
                _interfaceController.IncrementSelection(-1);
            else if (MouseState.CurrentState.ScrollWheelValue < MouseState.LastState.ScrollWheelValue)
                _interfaceController.IncrementSelection(1);

            Camera.Update();
        }
    }
}