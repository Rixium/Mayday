using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Blueprints;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Networking;
using Mayday.Game.Networking.Consumers;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Optimization;
using Mayday.Game.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Steamworks;
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
        private readonly GameScreenUserInterfaceController _interfaceController;
        
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
            foreach (var tile in GameWorld.Tiles) 
                SetupTile(tile);
        }

        private void OnTilePlaced(Tile tile)
        {
            SetupTile(tile);
            PacketManager.SendTileChangePacket(tile);
        }

        private void SetupTile(IEntity tile) => BluePrintManager.SetupFor(tile);

        public IEntity AddPlayer(IEntity player, bool isClients = false)
        {
            if (isClients)
            {
                var spawnTile = GameWorld.GetRandomSpawnLocation();
                player.X = spawnTile.TileX * GameWorld.TileSize;
                player.Y = spawnTile.TileY * GameWorld.TileSize - 70 * Game1.GlobalGameScale;
                MyPlayer = player;
            }

            player.GameWorld = GameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[1].Animations),
                BodyAnimator = new Animator(ContentChest.Bodies[1].Animations),
                LegsAnimator = new Animator(ContentChest.Legs[1].Animations)
            };
            
            var moveComponent = player.AddComponent(new MoveComponent());
            var gravityComponent = player.AddComponent(new GravityComponent());
            var jumpComponent = player.AddComponent(new JumpComponent());
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));
            var itemPickerComponent = player.AddComponent(new ItemPickerComponent(GameWorld.WorldItems, UpdateResolver));
            playerAnimationComponent = player.AddComponent(playerAnimationComponent);

            moveComponent.PositionChanged += PacketManager.SendPositionPacket;
            moveComponent.MoveDirectionChanged += PacketManager.SendMoveDirectionPacket;

            player.Bounds = new RectangleF(18 * Game1.GlobalGameScale, 18 * Game1.GlobalGameScale,
                42 * Game1.GlobalGameScale - 17 * Game1.GlobalGameScale - 18 * Game1.GlobalGameScale,
                33 * Game1.GlobalGameScale - 19 * Game1.GlobalGameScale);

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
                inventoryBar.AddItemToInventory(ContentChest.ItemData[ItemType.Shuttle]);
            }

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
        }

        private void SetupUiInput()
        {
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D1), () => _interfaceController.InventorySelectionChanged(0));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D2), () => _interfaceController.InventorySelectionChanged(1));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D3), () => _interfaceController.InventorySelectionChanged(2));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D4), () => _interfaceController.InventorySelectionChanged(3));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D5), () => _interfaceController.InventorySelectionChanged(4));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D6), () => _interfaceController.InventorySelectionChanged(5));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D7), () => _interfaceController.InventorySelectionChanged(6));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D8), () => _interfaceController.InventorySelectionChanged(7));
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.I), () => _interfaceController.ToggleMainInventory());
        }

        private void SetupWorldCallbacks() => GameWorld.TilePlaced += OnTilePlaced;

        private void SetupNetworking()
        {
            var gameServerListener = new MaydayServerNetworkListener(NetworkManager);
            var gameClientListener = new MaydayClientNetworkListener(NetworkManager);
            var consumers = new GamePacketConsumerManager(this);
            consumers.InjectInto(gameClientListener, gameServerListener);
        }
        
        public override void RenderScreen()
        {
            GraphicsUtils.Instance.Begin(true, Camera.GetMatrix());
            
            _worldRenderer.Draw(GameWorld, Camera);
            _playerRenderer.DrawPlayers(Players.GetAll());
            
            foreach (var entity in GameWorld.WorldItems.GetItems())
            {
                if (!UpdateResolver.ShouldUpdate(entity)) continue;
                if (!(entity is ItemDrop item)) continue;
                GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.ItemTextures[item.Item.ItemId], new Vector2(item.X, item.Y), Color.White);
            }
            
            GraphicsUtils.Instance.End();
        }

        public override void Update()
        {
            base.Update();
            
            NetworkManager?.Update();
            
            foreach(var player in Players.GetAll())
                player.Update();
            
            foreach(var entity in GameWorld.WorldItems.GetItems())
                entity.Update();

            if (MouseState.CurrentState.ScrollWheelValue > MouseState.LastState.ScrollWheelValue)
                _interfaceController.IncrementSelection(-1);
            else if (MouseState.CurrentState.ScrollWheelValue < MouseState.LastState.ScrollWheelValue)
                _interfaceController.IncrementSelection(1);
            
            Camera.Update();
        }

    }

}