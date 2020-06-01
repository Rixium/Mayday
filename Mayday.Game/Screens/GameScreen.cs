using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.UI;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.Listeners;
using Yetiface.Engine.Networking.Packagers;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {

        private readonly IWorldRenderer _worldRenderer;
        private readonly IPlayerRenderer _playerRenderer;
        
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        
        private IGameWorld _gameWorld;
        private readonly Camera _camera = new Camera();

        private Dictionary<ulong, IPlayer> _players;
        private IPlayer _myPlayer;

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _messagePackager = new MaydayMessagePackager();
            
            _worldRenderer = new WorldRenderer();
            _playerRenderer = new PlayerRenderer();
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
            player.Y = spawnTile.Y * _gameWorld.TileSize - 70;
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
                
            _players = new Dictionary<ulong, IPlayer> {
            {
                player.SteamId, player
            }};

            return player;
        }

        public override void Awake()
        {
            BackgroundColor = new Color(47, 39, 54);
            UserInterface = new GameScreenUserInterface(this, _networkManager);
            
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(-2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.Space), Jump);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(0), InputEventType.Released);

            _camera.SetEntity(_myPlayer);
        }

        private void Jump()
        {
            var inter = (GameScreenUserInterface) UserInterface;
            if (inter.TextInput.IsFocused) return;
            
            var jumpPacket = new JumpPacket
            {
                SteamId = _myPlayer.SteamId
            };

            var package = _messagePackager.Package(jumpPacket);
            _networkManager.SendMessage(package);
        }

        private Tile GetSpawnPosition() =>
            (from Tile tile in _gameWorld.Tiles
                where tile.TileType == TileType.GROUND 
                select _gameWorld.Tiles[(int) (_gameWorld.Width / 2.0f), tile.Y])
            .FirstOrDefault();

        private void Move(int x)
        {
            var inter = (GameScreenUserInterface) UserInterface;
            if (inter.TextInput.IsFocused) return;

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
                
                if (x == 0)
                {
                    var position = new PlayerPositionPacket
                    {
                        X = player.X,
                        Y = player.Y,
                        SteamId = SteamClient.SteamId
                    };

                    var package = _messagePackager.Package(position);
                    
                    _networkManager.SendMessage(package);
                }
                else
                {
                    player.FacingDirection = x;
                }
            }
            
            player.XDirection = x;
        }

        public override void Begin()
        {
        }

        /// <summary>
        /// Obviously a test implementation at the moment, so we can see
        /// the world rendering. Tbh ignore everything in this class, as it has just
        /// become a testing ground atm.
        /// </summary>
        public override void Draw()
        {
            UserInterface?.Draw();
            
            GraphicsUtils.Instance.SpriteBatch.GraphicsDevice.Clear(BackgroundColor);

            GraphicsUtils.Instance.Begin(true, _camera.GetMatrix());

            _worldRenderer.Draw(_gameWorld, _camera);
            _playerRenderer.DrawPlayers(_players);

            GraphicsUtils.Instance.End();
            
            UserInterface?.AfterDraw();
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
            
            if (MouseState.CurrentState.LeftButton == ButtonState.Pressed)
            {
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());
                var mouseTileX = mousePosition.X / _gameWorld.TileSize;
                var mouseTileY = mousePosition.Y / _gameWorld.TileSize;
                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > _gameWorld.Width - 1 ||
                    mouseTileY > _gameWorld.Height - 1) return;
                var tile = _gameWorld.Tiles[mouseTileX, mouseTileY];
                
                var oldType = tile.TileType;
                tile.TileType = TileType.GROUND;

                if (oldType != tile.TileType)
                {
                    SendTileChangePacket(tile);
                }
                
            } else if (MouseState.CurrentState.RightButton == ButtonState.Pressed)
            {
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());
                var mouseTileX = mousePosition.X / _gameWorld.TileSize;
                var mouseTileY = mousePosition.Y / _gameWorld.TileSize;
                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > _gameWorld.Width - 1 ||
                    mouseTileY > _gameWorld.Height - 1) return;
                var tile = _gameWorld.Tiles[mouseTileX, mouseTileY];

                var oldType = tile.TileType;
                tile.TileType = TileType.NONE;

                if (oldType != tile.TileType)
                {
                    SendTileChangePacket(tile);
                }
            }
            
            _camera.Update();
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

        public void OnNewConnection(Connection connection, ConnectionInfo info)
        {
            var spawnTile = GetSpawnPosition();

            var newPlayer = new Player
            {
                SteamId = info.Identity.SteamId,
                X = spawnTile.X * 16,
                Y = spawnTile.Y * 16- (int)(62 / 2f),
                GameWorld = _gameWorld
            };
            
            AddPlayer(newPlayer);
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