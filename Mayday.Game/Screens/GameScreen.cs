using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Packagers;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen
    {

        private readonly IWorldRenderer _worldRenderer;
        private readonly IPlayerRenderer _playerRenderer;
        
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        private Random _random = new Random();
        
        private IGameWorld _gameWorld;
        public Camera Camera { get; } = new Camera();

        private readonly Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();
        private Player _myPlayer;
        
        private GameScreenUserInterfaceController _interfaceController;

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _messagePackager = new MaydayMessagePackager();
            
            var gameServerListener = new MaydayServerNetworkListener(_messagePackager);
            var gameClientListener = new MaydayClientNetworkListener(_messagePackager);
            var consumers = new GamePacketConsumerManager(this, _players, _gameWorld);
            consumers.InjectInto(gameClientListener, gameServerListener);

            _networkManager.SetServerNetworkListener(gameServerListener);
            _networkManager.SetClientNetworkListener(gameClientListener);
            
            _worldRenderer = new WorldRenderer();
            _playerRenderer = new PlayerRenderer();

            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);
        }
        
        public void SetWorld(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;

            RegisterTileCallbacks();
        }

        private void RegisterTileCallbacks()
        {
            if (_gameWorld == null) return;
            foreach (var tile in _gameWorld.Tiles)
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
                GameWorld = _gameWorld
            };

            var moveComponent = itemDrop.AddComponent(new MoveComponent());
            itemDrop.AddComponent(new GravityComponent());
            moveComponent.XVelocity = _random.Next(-10, 10);
            moveComponent.YVelocity = _random.Next(0, 5);
            
            _gameWorld.WorldItems.Add(itemDrop);
            
            var tempTile = new Tile(0, tile.X, tile.Y);
            SendTileChangePacket(tempTile);
            SendItemDropPacket(itemDrop);
        }

        private void SendItemDropPacket(ItemDrop itemDrop)
        {
            var itemDropPacket = new ItemDropPacket()
            {
                X = itemDrop.X,
                Y = itemDrop.Y,
                ItemId = itemDrop.Item.Id
            };

            var package = _messagePackager.Package(itemDropPacket);
            
            _networkManager.SendMessage(package);
        }

        private void SendTileChangePacket(Tile tile)
        {
            var tileChangePacket = new TileTypePacket()
            {
                X = tile.X,
                Y = tile.Y,
                TileType = tile.TileType
            };

            var package = _messagePackager.Package(tileChangePacket);
            
            _networkManager.SendMessage(package);
        }
        
        public Player AddPlayer(Player player, bool isClients = false)
        {
            if (isClients)
            {
                _myPlayer = player;
                player.SteamId = GetId();
            }

            var spawnTile = GetSpawnPosition();
            
            player.X = spawnTile.X * _gameWorld.TileSize;
            player.Y = spawnTile.Y * _gameWorld.TileSize - 70 * Game1.GlobalGameScale;
            player.GameWorld = _gameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations),
                BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations),
                LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations)
            };
            
            player.AddComponent(new MoveComponent());
            player.AddComponent(playerAnimationComponent);
            player.AddComponent(new GravityComponent());
            player.AddComponent(new JumpComponent());
            player.AddComponent(new BlockBreakerComponent(_gameWorld, Camera));
            player.AddComponent(new ItemPickerComponent());
            
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));

            if (isClients)
            {
                inventoryBar.InventoryChanged += () => _interfaceController.InventoryBarChanged(inventoryBar);
                mainInventory.InventoryChanged += () => _interfaceController.MainInventoryChanged(mainInventory);
            }

            _players.Add(player.SteamId, player);

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
            
            YetiGame.InputManager.RegisterInputEvent("MoveRight", () => Move(2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent("MoveLeft", () => Move(-2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent("Jump", Jump);
            YetiGame.InputManager.RegisterInputEvent("MoveRight", () => Move(0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent("MoveLeft", () => Move(0), InputEventType.Released);

            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.I), _interfaceController.ToggleMainInventory);
            
            Camera.SetEntity(_myPlayer);
        }

        private void Jump()
        {
            var jumpPacket = new JumpPacket
            {
                SteamId = _myPlayer.SteamId
            };

            var package = _messagePackager.Package(jumpPacket);
            _networkManager.SendMessage(package);

            var jumpComponent = _myPlayer.GetComponent<JumpComponent>();
            jumpComponent.Jump();
        }

        private Tile GetSpawnPosition() =>
            (from Tile tile in _gameWorld.Tiles
                where tile.TileType == 1
                select _gameWorld.Tiles[(int) (_gameWorld.Width / 2.0f), tile.Y])
            .FirstOrDefault();

        private void Move(int x)
        {
            var player = _players[_myPlayer.SteamId];

            if (player.XDirection != x)
            {
                var data = new PlayerMovePacket()
                {
                    XDirection = x,
                    SteamId = player.SteamId
                };

                var movePackage = _messagePackager.Package(data);
                _networkManager.SendMessage(movePackage);
                
                var position = new PlayerPositionPacket
                {
                    X = (int) player.X,
                    Y = (int) player.Y,
                    SteamId = player.SteamId
                };

                var package = _messagePackager.Package(position);
                    
                _networkManager.SendMessage(package);
                
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
            
            _worldRenderer.Draw(_gameWorld, Camera);
            _playerRenderer.DrawPlayers(_players);

            foreach (var entity in _gameWorld.WorldItems)
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
            
            _networkManager?.Update();
            
            foreach(var player in _players)
                player.Value.Update();
            
            foreach(var entity in _gameWorld.WorldItems)
                entity.Update();
            
            Camera.Update();
        }
        
        public void DropItem(ItemDrop itemDrop)
        {
            itemDrop.GameWorld = _gameWorld;
            _gameWorld.WorldItems.Add(itemDrop);
        }
        
    }

}