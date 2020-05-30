using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeonBit.UI;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Graphics.Renderers;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Networking.Packets;
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
using Yetiface.Engine.UI;
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
        public Camera Camera = new Camera();
        
        private Dictionary<ulong, Player> Players { get; set; }
        public Player MyPlayer { get; set; }

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

        public override void Awake()
        {
            UserInterface = new GameUserInterface();
            
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(1), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(-1), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(0), InputEventType.Released);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(0), InputEventType.Released);

            var spawnTile = GetSpawnPosition();
            
            MyPlayer = new Player
            {
                SteamId = SteamClient.SteamId,
                X = spawnTile.X * DrawTileSize,
                Y = spawnTile.Y * DrawTileSize - (int)(62 / 2f)
            };
            
            MyPlayer.HeadAnimator = new Animator(ContentChest.Heads[MyPlayer.HeadId].Animations);
            MyPlayer.BodyAnimator = new Animator(ContentChest.Bodies[MyPlayer.BodyId].Animations);
            MyPlayer.ArmsAnimator = new Animator(ContentChest.Arms[MyPlayer.ArmsId].Animations);
            MyPlayer.LegsAnimator = new Animator(ContentChest.Legs[MyPlayer.LegsId].Animations);
            
            Players = new Dictionary<ulong, Player> {
            {
                MyPlayer.SteamId, MyPlayer
            }};

            BackgroundColor = Color.White;
            Camera.Goto(new Vector2(MyPlayer.X, MyPlayer.Y));
        }

        public int DrawTileSize { get; set; } = 12;

        private Tile GetSpawnPosition() =>
            (from Tile tile in _gameWorld.Tiles
                where tile.TileType == TileType.GROUND 
                select _gameWorld.Tiles[(int) (_gameWorld.Width / 2.0f), tile.Y])
            .FirstOrDefault();

        private void Move(int x)
        {
            var player = Players[SteamClient.SteamId];

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

            GraphicsUtils.Instance.Begin(true, Camera.GetMatrix());

            _worldRenderer.Draw(_gameWorld, Camera);
            _playerRenderer.DrawPlayers(Players);
            
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
            
            foreach(var player in Players)
                player.Value.Update();

            Camera.Goto(new Vector2(MyPlayer.X, MyPlayer.Y));
            
            if (MouseState.CurrentState.LeftButton == ButtonState.Pressed)
            {
                var mousePosition = MouseState.Bounds(Camera.GetMatrix());
                var mouseTileX = mousePosition.X / DrawTileSize;
                var mouseTileY = mousePosition.Y / DrawTileSize;
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
                var mousePosition = MouseState.Bounds(Camera.GetMatrix());
                var mouseTileX = mousePosition.X / DrawTileSize;
                var mouseTileY = mousePosition.Y / DrawTileSize;
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
                Y = spawnTile.Y * 16- (int)(62 / 2f)
            };
            
            newPlayer.HeadAnimator = new Animator(ContentChest.Heads[newPlayer.HeadId].Animations);
            newPlayer.BodyAnimator = new Animator(ContentChest.Bodies[newPlayer.BodyId].Animations);
            newPlayer.ArmsAnimator = new Animator(ContentChest.Arms[newPlayer.ArmsId].Animations);
            newPlayer.LegsAnimator = new Animator(ContentChest.Legs[newPlayer.LegsId].Animations);
            
            Players.Add(newPlayer.SteamId, newPlayer);
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
                var player = Players[movePacket.SteamId];
                var xDir = movePacket.XDirection;
                player.XDirection = xDir;
            } else if (received.GetType() == typeof(PlayerPositionPacket))
            {
                var positionPacket = (PlayerPositionPacket) received;
                var player = Players[positionPacket.SteamId];
                player.X = positionPacket.X;
                player.Y = positionPacket.Y;
            } 
            else if (received.GetType() == typeof(TileTypePacket))
            {
                var typePacket = (TileTypePacket) received;
                _gameWorld.Tiles[typePacket.X, typePacket.Y].TileType = typePacket.TileType;
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
                if (Players.ContainsKey(movePacket.SteamId))
                {
                    var player = Players[movePacket.SteamId];
                    player.XDirection = movePacket.XDirection;
                }
                else
                {
                    var spawnTile = GetSpawnPosition();
                    var player = new Player()
                    {
                        SteamId = movePacket.SteamId,
                        X = spawnTile.X * 16,
                        Y = spawnTile.Y * 16 - (int)(62 / 2f)
                    };

                    player.HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations);
                    player.BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations);
                    player.ArmsAnimator = new Animator(ContentChest.Arms[player.ArmsId].Animations);
                    player.LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations);
                    
                    Players.Add(player.SteamId, player);
                }
            } 
            else if (received.GetType() == typeof(PlayerPositionPacket))
            {
                var positionPacket = (PlayerPositionPacket) received;
                
                if (Players.ContainsKey(positionPacket.SteamId))
                {
                    var player = Players[positionPacket.SteamId];
                    player.X = positionPacket.X;
                    player.Y = positionPacket.Y;
                }
                else
                {
                    var player = new Player()
                    {
                        SteamId = positionPacket.SteamId,
                        X = positionPacket.X,
                        Y = positionPacket.Y
                    };

                    player.HeadAnimator = new Animator(ContentChest.Heads[player.HeadId].Animations);
                    player.BodyAnimator = new Animator(ContentChest.Bodies[player.BodyId].Animations);
                    player.ArmsAnimator = new Animator(ContentChest.Arms[player.ArmsId].Animations);
                    player.LegsAnimator = new Animator(ContentChest.Legs[player.LegsId].Animations);
                    
                    Players.Add(player.SteamId, player);
                }
            }
            else if (received.GetType() == typeof(TileTypePacket))
            {
                var typePacket = (TileTypePacket) received;
                _gameWorld.Tiles[typePacket.X, typePacket.Y].TileType = typePacket.TileType;
            }
        }

        public void OnConnectedToServer(ConnectionInfo info)
        {
            
        }
    }

    public class GameUserInterface : IUserInterface
    {

        public GameUserInterface()
        {
            UserInterface.Active.Clear();
            UserInterface.Active.UseRenderTarget = false;
        }
        
        public void Draw()
        {
            UserInterface.Active.Draw(GraphicsUtils.Instance.SpriteBatch);
        }

        public void Update()
        {
            UserInterface.Active.Update(Time.GameTime);
        }

        public void AfterDraw()
        {
            
        }
    }
}