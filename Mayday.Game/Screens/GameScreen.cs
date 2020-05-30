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
using Mayday.Game.Networking.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public Dictionary<int, int> TileBlobMap = new Dictionary<int, int>
        {
            {28, 0}, {124, 1}, {112, 2}, {16, 3}, {20, 4}, {116, 5}, {92, 6}, {80, 7}, {84, 8}, {221, 9}, 
            {31, 11}, {255, 12}, {241, 13}, {17, 14}, {23, 15}, {247, 16}, {223, 17}, {209, 18}, {215, 19}, {119, 20},
            {7, 22}, {199, 23}, {193, 24}, {1, 25}, {29, 26}, {253, 27}, {127, 28}, {113, 29}, {125, 30}, {93, 31}, {117, 32},
            {4, 33}, {68, 34}, {64, 35}, {0, 36}, {5, 37}, {197, 38}, {71, 39}, {65, 40}, {69, 41}, {87, 42}, {213, 43},
            {21, 48}, {245, 49}, {95, 50}, {81, 51}, {85, 52}
        };

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
            
            _messagePackager = new NetworkMessagePackager();
            _messagePackager.AddDefinition<TileTypePacket>();
            _messagePackager.AddDefinition<MapRequestPacket>();
            _messagePackager.AddDefinition<PlayerMovePacket>();
            _messagePackager.AddDefinition<PlayerPositionPacket>();
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

            DrawMap();
            
            foreach (var player in Players.Select(pair => pair.Value))
            {
                DrawPlayer(player);
            }
            
            GraphicsUtils.Instance.End();
            
            UserInterface?.Draw();
        }

        private void DrawMap()
        {
            var tileSize = 16;
            var startTileX = (int)(Camera.Position.X - Window.ViewportWidth / 2.0f) / DrawTileSize - 1;
            var startTileY = (int)(Camera.Position.Y - Window.ViewportHeight / 2.0f) / DrawTileSize - 1;
            var endTileX = (int)(Camera.Position.X + Window.ViewportWidth / 2.0f) / DrawTileSize - 1;
            var endTileY = (int)(Camera.Position.Y + Window.ViewportHeight / 2.0f) / DrawTileSize - 1;

            for (var i = startTileX; i < endTileX; i++)
            {
                for (var j = startTileY; j < endTileY; j++)
                {
                    if (i < 0 || j < 0 || i > _gameWorld.Width - 1 || j > _gameWorld.Height - 1) continue;
                    var tile = _gameWorld.Tiles[i, j];
                    
                    if (tile.TileType == TileType.NONE) continue;

                
                    var blobSelect = GetTileNumberFor(tile);

                    TileBlobMap.TryGetValue(blobSelect, out var tileNumber);

                    if (tileNumber == -1)
                        tileNumber = 36;
                
                    var tileSet = ContentChest.Tiles[(int) tile.TileType];
                
                    IndexConvert(tileNumber, tileSet.Width / tileSize, out var x, out var y);
                    var rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet, 
                        new Rectangle(tile.X * DrawTileSize, tile.Y * DrawTileSize, DrawTileSize, DrawTileSize), 
                        rect, Color.White);
                }
            }
        }

        private int GetTileNumberFor(Tile tile)
        {
            var x = tile.X;
            var y = tile.Y;
            byte bitSum = 0;

            var n = TryGetTile(tile.X, tile.Y - 1);
            var e  = TryGetTile(tile.X + 1, tile.Y);
            var s = TryGetTile(tile.X, tile.Y + 1);
            var w = TryGetTile(tile.X - 1, tile.Y);
            var nw = TryGetTile(tile.X - 1, tile.Y - 1);
            var ne = TryGetTile(tile.X + 1, tile.Y - 1);
            var se = TryGetTile(tile.X + 1, tile.Y + 1);
            var sw = TryGetTile(tile.X - 1, tile.Y + 1);

            TileTypeMatch(ref bitSum, tile, n, 1);
            TileTypeMatch(ref bitSum, tile, e, 4);
            TileTypeMatch(ref bitSum, tile, s, 16);
            TileTypeMatch(ref bitSum, tile, w, 64);
            TileTypeMatch(ref bitSum, tile, se, 8, e, s);
            TileTypeMatch(ref bitSum, tile, ne, 2, n, e);
            TileTypeMatch(ref bitSum, tile, sw, 32, s, w);
            TileTypeMatch(ref bitSum, tile, nw, 128, n, w);
            
            return bitSum;
        }

        private Tile TryGetTile(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0) return null;
            if (tileX > _gameWorld.Width - 1 || tileY > _gameWorld.Height - 1) return null;
            return _gameWorld.Tiles[tileX, tileY];
        }

        public void IndexConvert(int index, int arrayWidth, out int x, out int y)
        {
            x = index % arrayWidth;
            y = index / arrayWidth;
        }

        private void TileTypeMatch(ref byte bitSum, Tile tile1, Tile tile2, byte bitValue, params Tile[] assureNot)
        {
            if (tile2 == null) return;
            if (tile1 == null) return;

            if (tile1.TileType.Equals(tile2.TileType))
            {
                foreach(var tile in assureNot)
                    if (tile.TileType != tile1.TileType)
                        return;
                
                bitSum += bitValue;
            }
        }

        private void DrawPlayer(Player player)
        {
            var headSprite = player.HeadAnimator?.Current;
            var bodySprite = player.BodyAnimator?.Current;
            var armSprite = player.ArmsAnimator?.Current;
            var legSprite = player.LegsAnimator?.Current;
            var playerPosition = new Vector2(player.X, player.Y);

            if(armSprite != null)
                GraphicsUtils.Instance.SpriteBatch.Draw(
                    armSprite.Texture, playerPosition, armSprite.SourceRectangle, Color.White,
                    0, armSprite.Origin, 1f, SpriteEffects.FlipHorizontally, 0F);
            if (legSprite != null)
                GraphicsUtils.Instance.Draw(legSprite, playerPosition, Color.White);
            if(bodySprite != null)
                GraphicsUtils.Instance.Draw(bodySprite, playerPosition, Color.White);
            if(headSprite != null)
                GraphicsUtils.Instance.Draw(headSprite, playerPosition, Color.White);
            if(armSprite != null)
                GraphicsUtils.Instance.Draw(armSprite, playerPosition, Color.White);

            var name = SteamFriends.GetFriendPersona(player.SteamId);
            var nameSize = GraphicsUtils.Instance.DebugFont.MeasureString(name);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont,
                name, new Vector2(player.X + headSprite.Texture.Width / 2.0f - nameSize.X / 2.0f, player.Y - 20 - nameSize.Y), Color.Black, 0, new Vector2(nameSize.X / 2.0f, nameSize.
                    Y / 2.0f), 1, SpriteEffects.None, 0F);
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