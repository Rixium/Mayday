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
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.UI.Controllers;
using Mayday.UI.Views;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.TextureAtlases;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {

        private readonly IWorldRenderer _worldRenderer;
        private readonly IPlayerRenderer _playerRenderer;
        
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        
        private IGameWorld _gameWorld;
        public Camera Camera { get; } = new Camera();

        private readonly Dictionary<ulong, IPlayer> _players = new Dictionary<ulong, IPlayer>();
        private IPlayer _myPlayer;
        
        private GameScreenUserInterfaceController _interfaceController;

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _messagePackager = new MaydayMessagePackager();
            
            _worldRenderer = new WorldRenderer();
            _playerRenderer = new PlayerRenderer();

            var gameScreenUserInterface = new GameScreenUserInterface();
            _interfaceController = new GameScreenUserInterfaceController(gameScreenUserInterface);
            UserInterface = new MyraUserInterface(gameScreenUserInterface);
        }
        
        public void SetWorld(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }
        
        public IPlayer AddPlayer(IPlayer player, bool isClients = false)
        {
            if (isClients)
            {
                _myPlayer = player;
                player.SteamId = SteamClient.SteamId;
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
            player.AddComponent(new BlockBreakerComponent(_gameWorld, Camera, _networkManager));
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
            UserInterface.SetActive();
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
            var player = _players[SteamClient.SteamId];

            if (player.XDirection != x)
            {
                var data = new PlayerMovePacket()
                {
                    XDirection = x,
                    SteamId = SteamClient.SteamId
                };

                var movePackage = _messagePackager.Package(data);
                _networkManager.SendMessage(movePackage);
                
                var position = new PlayerPositionPacket
                {
                    X = (int) player.X,
                    Y = (int) player.Y,
                    SteamId = SteamClient.SteamId
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
            
            Camera.Update();
        }

        public void OnNewConnection(Connection connection, ConnectionInfo info)
        {
            
        }


        public void OnConnectionLeft(Connection connection, ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum,
            long recvTime, int channel)
        {
            var received = _messagePackager.Unpack(data, size);

            if (received.GetType() == typeof(MapRequestPacket))
            {
                SendMap();
            } else if (received.GetType() == typeof(PlayerMovePacket))
            {
                var movePacket = (PlayerMovePacket) received;
                var player = _players[movePacket.SteamId];
                var xDir = movePacket.XDirection;
                player.XDirection = xDir;
                player.FacingDirection = movePacket.XDirection != 0 ? movePacket.XDirection :
                    player.FacingDirection;
            } else if (received.GetType() == typeof(PlayerPositionPacket))
            {
                var positionPacket = (PlayerPositionPacket) received;
                var player = _players[positionPacket.SteamId];
                player.X = positionPacket.X;
                player.Y = positionPacket.Y;
            } 
            else if (received.GetType() == typeof(TileTypePacket))
            {
                var typePacket = (TileTypePacket) received;
                _gameWorld.Tiles[typePacket.X, typePacket.Y].TileType = typePacket.TileType;
            } else if (received.GetType() == typeof(JumpPacket))
            {
                var jump = (JumpPacket) received;
                var player = _players[jump.SteamId];
                var jumpComponent = player.GetComponent<JumpComponent>();
                jumpComponent.Jump();
            } else if (received.GetType() == typeof(ChatMessagePacket))
            {
                AddMessageToChat((ChatMessagePacket) received);
            } else if (received.GetType() == typeof(NewPlayerPacket))
            {
                var newPlayer = (NewPlayerPacket) received;
                
                var player = new Player
                {
                    SteamId = newPlayer.SteamId,
                    X = newPlayer.X,
                    Y = newPlayer.Y,
                    GameWorld = _gameWorld,
                    HeadId = newPlayer.HeadId
                };

                AddPlayer(player);
            }
        }

        private void AddMessageToChat(ChatMessagePacket received)
        {
            _players.TryGetValue(received.SteamId, out var selectedPlayer);
        }

        private async void SendMap()
        {
            for (var i = 0; i < _gameWorld.Width; i++)
            {
                for (var j = 0; j < _gameWorld.Height; j++)
                {
                    var tileToSend = _gameWorld.Tiles[i, j];
                    var tileTypePacket = new TileTypePacket()
                    {
                        X = tileToSend.X,
                        Y = tileToSend.Y,
                        TileType = tileToSend.TileType
                    };
                    var packet = _messagePackager.Package(tileTypePacket);
                    _networkManager.SendMessage(packet);
                }

                await Task.Delay(1);
            }
            
        }

        public void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            
        }


        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var received = _messagePackager.Unpack(data, size);

            if (received.GetType() == typeof(PlayerMovePacket))
            {
                var movePacket = (PlayerMovePacket) received;
                if (_players.ContainsKey(movePacket.SteamId))
                {
                    var player = _players[movePacket.SteamId];
                    player.XDirection = movePacket.XDirection;
                    player.FacingDirection = movePacket.XDirection != 0 ? movePacket.XDirection :
                    player.FacingDirection;
                }
                else
                {
                    var spawnTile = GetSpawnPosition();
                    var player = new Player()
                    {
                        SteamId = movePacket.SteamId,
                        X = spawnTile.X * 16,
                        Y = spawnTile.Y * 16 - (int)(62 / 2f),
                        GameWorld = _gameWorld
                    };

                    AddPlayer(player);
                }
            } 
            else if (received.GetType() == typeof(PlayerPositionPacket))
            {
                var positionPacket = (PlayerPositionPacket) received;
                
                if (_players.ContainsKey(positionPacket.SteamId))
                {
                    var player = _players[positionPacket.SteamId];
                    player.X = positionPacket.X;
                    player.Y = positionPacket.Y;
                }
                else
                {
                    var player = new Player()
                    {
                        SteamId = positionPacket.SteamId,
                        X = positionPacket.X,
                        Y = positionPacket.Y,
                        GameWorld = _gameWorld
                    };

                    AddPlayer(player);
                }
            }
            else if (received.GetType() == typeof(TileTypePacket))
            {
                var typePacket = (TileTypePacket) received;
                _gameWorld.Tiles[typePacket.X, typePacket.Y].TileType = typePacket.TileType;
            } else if (received.GetType() == typeof(JumpPacket))
            {
                var jump = (JumpPacket) received;
                var player = _players[jump.SteamId];
                var jumpComponent = player.GetComponent<JumpComponent>();
                jumpComponent.Jump();
            } else if (received.GetType() == typeof(ChatMessagePacket))
            {
                AddMessageToChat((ChatMessagePacket) received);
            } else if (received.GetType() == typeof(NewPlayerPacket))
            {
                var newPlayer = (NewPlayerPacket) received;
                
                var player = new Player()
                {
                    SteamId = newPlayer.SteamId,
                    X = newPlayer.X,
                    Y = newPlayer.Y,
                    GameWorld = _gameWorld,
                    HeadId = newPlayer.HeadId
                };

                AddPlayer(player);
            }
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }

        public void AddChat(ChatMessagePacket chatMessage) => AddMessageToChat(chatMessage);
    }

}