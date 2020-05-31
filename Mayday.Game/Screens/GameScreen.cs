using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
using Mayday.Game.UI;
using Microsoft.Xna.Framework;
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
        
        public void SetPlayer(IPlayer player)
        {
            _myPlayer = player;
            
            _players = new Dictionary<ulong, IPlayer> {
            {
                player.SteamId, player
            }};
            
            _myPlayer.HeadAnimator = new Animator(ContentChest.Heads[_myPlayer.HeadId].Animations);
            _myPlayer.BodyAnimator = new Animator(ContentChest.Bodies[_myPlayer.BodyId].Animations);
            _myPlayer.ArmsAnimator = new Animator(ContentChest.Arms[_myPlayer.ArmsId].Animations);
            _myPlayer.LegsAnimator = new Animator(ContentChest.Legs[_myPlayer.LegsId].Animations);
        }

        public override void Awake()
        {
            UserInterface = new GameScreenUserInterface();
            
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(-2), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.Space), Jump);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(0), InputEventType.Released);
            
            var spawnTile = GetSpawnPosition();

            SetPlayer(new Player
            {
                SteamId = SteamClient.SteamId,
                X = spawnTile.X * _gameWorld.TileSize,
                Y = spawnTile.Y * _gameWorld.TileSize - 70,
                GameWorld = _gameWorld
            });

            BackgroundColor = Color.White;
            _camera.Position = (new Vector2(_myPlayer.X, _myPlayer.Y - 100));
        }

        private void Jump()
        {
            var jumpPacket = new JumpPacket
            {
                SteamId = _myPlayer.SteamId
            };

            var package = _messagePackager.Package(jumpPacket);
            _networkManager.SendMessage(package);
            
            _myPlayer.Jump();
        }

        private Tile GetSpawnPosition() =>
            (from Tile tile in _gameWorld.Tiles
                where tile.TileType == TileType.GROUND 
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
            GraphicsUtils.Instance.SpriteBatch.GraphicsDevice.Clear(BackgroundColor);

            GraphicsUtils.Instance.Begin(true, _camera.GetMatrix());

            _worldRenderer.Draw(_gameWorld, _camera);
            _playerRenderer.DrawPlayers(_players);

            GraphicsUtils.Instance.End();
            
            UserInterface?.Draw();
        }

        public override void Finish()
        {
        }

        public override void Update()
        {
            _networkManager?.Update();
            UserInterface?.Update();
            
            foreach(var player in _players)
                player.Value.Update();

            _camera.Goto(new Vector2(_myPlayer.X, _myPlayer.Y));
            
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
            
            newPlayer.HeadAnimator = new Animator(ContentChest.Heads[newPlayer.HeadId].Animations);
            newPlayer.BodyAnimator = new Animator(ContentChest.Bodies[newPlayer.BodyId].Animations);
            newPlayer.ArmsAnimator = new Animator(ContentChest.Arms[newPlayer.ArmsId].Animations);
            newPlayer.LegsAnimator = new Animator(ContentChest.Legs[newPlayer.LegsId].Animations);
            
            _players.Add(newPlayer.SteamId, newPlayer);
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
                var jumpPacket = (JumpPacket) received;
                _players[jumpPacket.SteamId].Jump();
            }
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

                    player.HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations);
                    player.BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations);
                    player.ArmsAnimator = new Animator(ContentChest.Arms[player.ArmsId].Animations);
                    player.LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations);
                    
                    _players.Add(player.SteamId, player);
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

                    player.HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations);
                    player.BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations);
                    player.ArmsAnimator = new Animator(ContentChest.Arms[player.ArmsId].Animations);
                    player.LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations);
                    
                    _players.Add(player.SteamId, player);
                }
            }
            else if (received.GetType() == typeof(TileTypePacket))
            {
                var typePacket = (TileTypePacket) received;
                _gameWorld.Tiles[typePacket.X, typePacket.Y].TileType = typePacket.TileType;
            } else if (received.GetType() == typeof(JumpPacket))
            {
                var jumpPacket = (JumpPacket) received;
                _players[jumpPacket.SteamId].Jump();
            }
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }
    }

}