using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Networking.Consumers;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Packagers;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen
    {

        public INetworkManager NetworkManager { get; set; }
        public INetworkMessagePackager MessagePackager { get; set; }
        public IGameWorld GameWorld { get; set; }
        public Dictionary<ulong, Player> Players { get; set; } 
            = new Dictionary<ulong, Player>();
        
        private readonly IWorldRenderer _worldRenderer;
        private readonly IPlayerRenderer _playerRenderer;
        private Random _random = new Random();
        
        public Camera Camera { get; } = new Camera();

        public Player MyPlayer { get; set; }
        
        private GameScreenUserInterfaceController _interfaceController;

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            NetworkManager = networkManager;
            MessagePackager = new MaydayMessagePackager();
            
            _worldRenderer = new WorldRenderer();
            _playerRenderer = new PlayerRenderer();
            
            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);
        }
        
        public void SetWorld(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;

            RegisterTileCallbacks();
        }

        private void RegisterTileCallbacks()
        {
            if (GameWorld == null) return;
            foreach (var tile in GameWorld.Tiles)
            {
                tile.TileDestroyed += OnTileDestroyed;
            }
        }

        private void OnTileDestroyed(Tile tile)
        {
            var itemDrop = new ItemDrop
            {
                Item = ContentChest.ItemData[tile.TileProperties.ItemDropId],
                X = tile.RenderX,
                Y = tile.RenderY,
                GameWorld = GameWorld
            };

            var moveComponent = itemDrop.AddComponent(new MoveComponent());
            itemDrop.AddComponent(new GravityComponent());
            moveComponent.XVelocity = _random.Next(-10, 10);
            moveComponent.YVelocity = _random.Next(0, 5);
            
            GameWorld.WorldItems.Add(itemDrop);
            
            var tempTile = new Tile(0, tile.X, tile.Y);
            SendTileChangePacket(tempTile);
            SendItemDropPacket(itemDrop);
        }
        
        private void OnTilePlaced(Tile tile)
        {
            SendTileChangePacket(tile);
        }

        private void SendItemDropPacket(ItemDrop itemDrop)
        {
            var itemDropPacket = new ItemDropPacket()
            {
                X = itemDrop.X,
                Y = itemDrop.Y,
                ItemId = itemDrop.Item.Id
            };

            var package = MessagePackager.Package(itemDropPacket);
            
            NetworkManager.SendMessage(package);
        }

        private void SendTileChangePacket(Tile tile)
        {
            var tileChangePacket = new TileTypePacket()
            {
                X = tile.X,
                Y = tile.Y,
                TileType = tile.TileType
            };

            var package = MessagePackager.Package(tileChangePacket);
            
            NetworkManager.SendMessage(package);
        }
        
        public Player AddPlayer(Player player, bool isClients = false)
        {
            if (isClients)
            {
                var spawnTile = GetSpawnPosition();
                player.X = spawnTile.X * GameWorld.TileSize;
                player.Y = spawnTile.Y * GameWorld.TileSize - 70 * Game1.GlobalGameScale;
                player.SteamId = GetId();
                MyPlayer = player;
            }

            player.GameWorld = GameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations),
                BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations),
                LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations)
            };
            
            player.AddComponent(new MoveComponent());
            player.AddComponent(playerAnimationComponent);
            player.AddComponent(new GravityComponent());
            player.AddComponent(new JumpComponent(this));
            var blockBreakerComponent = player.AddComponent(new BlockBreakerComponent(GameWorld, Camera));
            player.AddComponent(new ItemPickerComponent());

            OnMouseDown += blockBreakerComponent.MouseDown;
            
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));

            if (isClients)
            {
                inventoryBar.InventoryChanged += () => _interfaceController.InventoryBarChanged(inventoryBar);
                mainInventory.InventoryChanged += () => _interfaceController.MainInventoryChanged(mainInventory);
            }

            Players.Add(player.SteamId, player);

            return player;
        }

        private ulong GetId()
        {
            try
            {
                return SteamClient.SteamId;
            }
            catch (Exception _)
            {
                return 0;
            }
        }

        public override void Awake()
        {
            BackgroundColor = new Color(47, 39, 54);

            SetupNetworking();
            SetupWorldCallbacks();
            
            YetiGame.InputManager.RegisterInputEvent("MoveRight", () => Move(2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent("MoveLeft", () => Move(-2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent("Jump", Jump);
            YetiGame.InputManager.RegisterInputEvent("MoveRight", () => Move(0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent("MoveLeft", () => Move(0), InputEventType.Released);

            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.I), _interfaceController.ToggleMainInventory);
            
            Camera.SetEntity(MyPlayer);
        }

        private void SetupWorldCallbacks()
        {
            GameWorld.TilePlaced += OnTilePlaced;
        }

        private void SetupNetworking()
        {            
            var gameServerListener = new MaydayServerNetworkListener(NetworkManager, MessagePackager);
            var gameClientListener = new MaydayClientNetworkListener(MessagePackager);
            
            NetworkManager.SetServerNetworkListener(gameServerListener);
            NetworkManager.SetClientNetworkListener(gameClientListener);
            
            var consumers = new GamePacketConsumerManager(this);
            consumers.InjectInto(gameClientListener, gameServerListener);
        }

        private void Jump()
        {
            var jumpPacket = new JumpPacket
            {
                SteamId = MyPlayer.SteamId
            };

            var package = MessagePackager.Package(jumpPacket);
            NetworkManager.SendMessage(package);

            var jumpComponent = MyPlayer.GetComponent<JumpComponent>();
            jumpComponent.Jump();
        }

        private Tile GetSpawnPosition() =>
            (from Tile tile in GameWorld.Tiles
                where tile.TileType == 1
                select GameWorld.Tiles[(int) (GameWorld.Width / 2.0f), tile.Y])
            .FirstOrDefault();

        private void Move(int x)
        {
            var player = Players[MyPlayer.SteamId];

            if (player.XDirection != x)
            {
                var data = new PlayerMovePacket()
                {
                    XDirection = x,
                    SteamId = player.SteamId
                };

                var movePackage = MessagePackager.Package(data);
                NetworkManager.SendMessage(movePackage);
                
                var position = new PlayerPositionPacket
                {
                    X = (int) player.X,
                    Y = (int) player.Y,
                    SteamId = player.SteamId
                };

                var package = MessagePackager.Package(position);
                    
                NetworkManager.SendMessage(package);
                
                if (x != 0)
                {
                    player.FacingDirection = x;
                }
            }
            
            player.XDirection = x;
        }

        public override void Begin()
        {
        }
        
        public override void RenderScreen()
        {
            GraphicsUtils.Instance.Begin(true, Camera.GetMatrix());
            
            _worldRenderer.Draw(GameWorld, Camera);
            _playerRenderer.DrawPlayers(Players);

            foreach (var entity in GameWorld.WorldItems)
            {
                if (!Camera.Intersects(entity.GetBounds())) continue;
                if (entity.GetType() != typeof(ItemDrop)) continue;
                var item = (ItemDrop) entity;
                GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.Items[item.Item.Id], new Vector2(item.X, item.Y), Color.White);
            }
            
            GraphicsUtils.Instance.End();
        }

        public override void Finish()
        {
        }

        public override void Update()
        {
            base.Update();
            
            NetworkManager?.Update();
            
            foreach(var player in Players)
                player.Value.Update();
            
            foreach(var entity in GameWorld.WorldItems)
                entity.Update();
            
            Camera.Update();
        }
        
        public void DropItem(ItemDrop itemDrop)
        {
            itemDrop.GameWorld = GameWorld;
            GameWorld.WorldItems.Add(itemDrop);
        }

        public void SendPacket(JumpPacket jumpPacket)
        {
            jumpPacket.SteamId = SteamClient.SteamId;
            var package = MessagePackager.Package(jumpPacket);
            NetworkManager.SendMessage(package);
        }
        
    }

}