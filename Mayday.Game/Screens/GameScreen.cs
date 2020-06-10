using System.Linq;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Networking.Consumers;
using Mayday.Game.Networking.Listeners;
using Mayday.Game.Networking.Packets;
using Mayday.Game.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework;
using Steamworks;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen
    {
        public INetworkManager NetworkManager { get; }
        public IGameWorld GameWorld { get; private set; }

        public readonly IPlayerSet Players = new PlayerSet();
        public Player MyPlayer { get; private set; }

        private readonly IWorldRenderer _worldRenderer;
        private readonly IPlayerRenderer _playerRenderer;
        private readonly GameScreenUserInterfaceController _interfaceController;

        private Camera Camera { get; } = new Camera();

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            NetworkManager = networkManager;
            
            _worldRenderer = new WorldRenderer();
            _playerRenderer = new PlayerRenderer();
            
            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);
        }
        
        public void SetWorld(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }

        private void SetupTiles()
        {
            foreach (var tile in GameWorld.Tiles)
            {
                var itemDropId = tile.TileProperties?.ItemDropId;

                if (itemDropId == null || itemDropId.Value == -1) continue;
                
                var itemDropperComponent = tile.AddComponent(
                    new ItemDropperComponent(itemDropId.Value));

                itemDropperComponent.ItemDrop += DropItem;
            }
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

            var package = NetworkManager.MessagePackager.Package(itemDropPacket);
            
            NetworkManager.SendMessage(package);
        }

        private void SendTileChangePacket(Tile tile)
        {
            var tileChangePacket = new TileTypePacket()
            {
                X = tile.TileX,
                Y = tile.TileY,
                TileType = tile.TileType
            };

            var package = NetworkManager.MessagePackager.Package(tileChangePacket);
            
            NetworkManager.SendMessage(package);
        }
        
        public Player AddPlayer(Player player, bool isClients = false)
        {
            if (isClients)
            {
                var spawnTile = GetSpawnPosition();
                player.X = spawnTile.TileX * GameWorld.TileSize;
                player.Y = spawnTile.TileY * GameWorld.TileSize - 70 * Game1.GlobalGameScale;
                player.SetClientId();
                MyPlayer = player;
            }

            player.GameWorld = GameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations),
                BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations),
                LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations)
            };
            
            var moveComponent = player.AddComponent(new MoveComponent());
            playerAnimationComponent = player.AddComponent(playerAnimationComponent);
            var gravityComponent = player.AddComponent(new GravityComponent());
            var jumpComponent = player.AddComponent(new JumpComponent(this));
            var blockBreakerComponent = player.AddComponent(new BlockBreakerComponent(GameWorld, Camera));
            var itemPickerComponent = player.AddComponent(new ItemPickerComponent());

            OnMouseDown += blockBreakerComponent.MouseDown;
            moveComponent.PositionChanged += SendPositionPacket;
            moveComponent.MoveDirectionChanged += SendMoveDirectionPacket;
            
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));

            if (isClients)
            {
                player.AddComponent(new CharacterControllerComponent());
                inventoryBar.InventoryChanged += () => _interfaceController.InventoryBarChanged(inventoryBar);
                mainInventory.InventoryChanged += () => _interfaceController.MainInventoryChanged(mainInventory);
                jumpComponent.Jump += SendJumpPacket;
            }

            Players.Add(player);

            return player;
        }

        private void SendJumpPacket(JumpComponent jumpComponent)
        {
            var jumpPacket = new JumpPacket
            {
                SteamId = MyPlayer.SteamId
            };

            var package = NetworkManager.MessagePackager.Package(jumpPacket);
            NetworkManager.SendMessage(package);
        }

        public override void Awake()
        {
            BackgroundColor = new Color(47, 39, 54);

            SetupNetworking();
            SetupWorldCallbacks();
            SetupTiles();
            Camera.SetEntity(MyPlayer);
        }

        private void SetupWorldCallbacks()
        {
            GameWorld.TilePlaced += OnTilePlaced;
        }

        private void SetupNetworking()
        {            
            var gameServerListener = new MaydayServerNetworkListener(NetworkManager);
            var gameClientListener = new MaydayClientNetworkListener(NetworkManager);
            
            NetworkManager.SetServerNetworkListener(gameServerListener);
            NetworkManager.SetClientNetworkListener(gameClientListener);
            
            var consumers = new GamePacketConsumerManager(this);
            consumers.InjectInto(gameClientListener, gameServerListener);
        }
        
        private Tile GetSpawnPosition() =>
            (from Tile tile in GameWorld.Tiles
                where tile.TileType == 1
                select GameWorld.Tiles[(int) (GameWorld.Width / 2.0f), tile.TileY])
            .FirstOrDefault();

        private void SendPositionPacket(IComponent moveComponent)
        {
            var entity = moveComponent.Entity;
            
            var position = new PlayerPositionPacket
            {
                X = (int) entity.X,
                Y = (int) entity.Y,
                SteamId = MyPlayer.SteamId
            };

            var package = NetworkManager.MessagePackager.Package(position);
                
            NetworkManager.SendMessage(package);
        }

        private void SendMoveDirectionPacket(IComponent moveComponent)
        {
            var entity = moveComponent.Entity;
            
            var data = new PlayerMovePacket()
            {
                XDirection = entity.XDirection,
                SteamId = MyPlayer.SteamId
            };
            
            var movePackage = NetworkManager.MessagePackager.Package(data);
            NetworkManager.SendMessage(movePackage);
        }

        public override void Begin()
        {
        }
        
        public override void RenderScreen()
        {
            GraphicsUtils.Instance.Begin(true, Camera.GetMatrix());
            
            _worldRenderer.Draw(GameWorld, Camera);
            _playerRenderer.DrawPlayers(Players.GetAll());

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
            
            foreach(var player in Players.GetAll())
                player.Update();
            
            foreach(var entity in GameWorld.WorldItems)
                entity.Update();
            
            Camera.Update();
        }
        
        public void DropItem(ItemDrop itemDrop)
        {
            itemDrop.GameWorld = GameWorld;
            GameWorld.WorldItems.Add(itemDrop);
            SendItemDropPacket(itemDrop);
        }

        public void SendPacket(JumpPacket jumpPacket)
        {
            jumpPacket.SteamId = SteamClient.SteamId;
            var package = NetworkManager.MessagePackager.Package(jumpPacket);
            NetworkManager.SendMessage(package);
        }
        
    }

}