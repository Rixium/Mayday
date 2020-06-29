﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
using Microsoft.Xna.Framework.Graphics;
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
        private readonly LightMapRenderer _lightMapRenderer = new LightMapRenderer();
        private readonly LightMap _lightMap = new LightMap();
        private readonly GameScreenUserInterfaceController _interfaceController;
        private readonly HashSet<IRenderable> _renderableComponents = new HashSet<IRenderable>();

        private float[,] Lights
        {
            get => _lights;
            set
            {
                _lights = value;
                _lightsChanged = true;
            }
        }
        private bool _lightsChanged;
        private float[,] _lights;
        private RenderTarget2D _renderTarget = new RenderTarget2D(
            Window.GraphicsDeviceManager.GraphicsDevice,
            Window.WindowWidth, Window.WindowHeight);

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
            PacketManager.SendTileChangePacket(tile);

            RecalculateLighting();
        }

        private async void RecalculateLighting()
        {
            Lights = await Task.Run(async () =>
            {
                var data = await _lightMap.CheckLights(MyPlayer.GameArea);
                return data;
            });
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

        public override void RenderScreen()
        {
            if(_renderTarget.Width != Window.WindowWidth)
                _renderTarget = new RenderTarget2D(
                Window.GraphicsDeviceManager.GraphicsDevice,
                Window.WindowWidth, Window.WindowHeight);

            if (_lightsChanged)
            {
                _lightMapRenderer.RenderToRenderTarget(Lights);
                _lightsChanged = false;
            }

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsUtils.Instance.Begin();
            GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.Background, new Rectangle(0, 0, Window.WindowWidth, Window.WindowHeight), Color.White);
            GraphicsUtils.Instance.End();

            GraphicsUtils.Instance.Begin(Camera.GetMatrix());
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

            GraphicsUtils.Instance.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,  // No blending
                null, // Point clamp, so we get sexy pixel perfect resizing
                      null, // We don't care about this. Tbh, I don't even understand it.
                null, // I don't even know what this it.
                null, // We can choose to flip textures as an example, but we dont, so null it.
                Camera.GetMatrix()); // Window viewport, for nice resizing.
            _lightMapRenderer?.Draw(MyPlayer, GameWorld);
            GraphicsUtils.Instance.End();

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            GraphicsUtils.Instance.SpriteBatch.Begin();
            GraphicsUtils.Instance.SpriteBatch.Draw(_renderTarget,
                new Rectangle(0, 0, Window.WindowWidth, Window.WindowHeight), Color.White);
            GraphicsUtils.Instance.End();
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

            Camera.Update(MyPlayer.GameArea.AreaWidth * GameWorld.TileSize, MyPlayer.GameArea.AreaHeight * GameWorld.TileSize);
        }
    }
}