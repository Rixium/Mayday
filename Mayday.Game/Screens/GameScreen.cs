using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeonBit.UI;
using Mayday.Game.Gameplay;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers;
using Mayday.Game.Networking;
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

namespace Mayday.Game.Screens
{

    public class Player
    {
        public uint SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {

        private Dictionary<uint, Player> Players { get; set; }
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        
        private IGameWorld _gameWorld;

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _messagePackager = new NetworkMessagePackager();
            _messagePackager.AddDefinition<TileTypePacket>();
            _messagePackager.AddDefinition<MapRequestPacket>();
            _messagePackager.AddDefinition<PlayerMovePacket>();

            Players = new Dictionary<uint, Player> {
            {
                (uint) SteamClient.SteamId, new Player()
                {
                    SteamId = (uint) SteamClient.SteamId
                }
            }};
        }

        public void SetWorld(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public override void Awake()
        {
            UserInterface = new GameUserInterface();
            
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(1, 0), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(-1, 0), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.W), () => Move(0, -1), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.S), () => Move(0, 1), InputEventType.Held);
        }

        private void Move(int x, int y)
        {
            var myPlayer = Players[(uint) SteamClient.SteamId];
            myPlayer.X += x;
            myPlayer.Y += y;

            var packet = _messagePackager.Package(new PlayerMovePacket()
            {
                SteamId = (uint) SteamClient.SteamId,
                X = myPlayer.X,
                Y = myPlayer.Y
            });
            
            _networkManager.SendMessage(packet);
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
            GraphicsUtils.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Black);
            GraphicsUtils.Instance.Begin();

            foreach (var tile in _gameWorld.Tiles)
            {
                if (tile.TileType == TileType.NONE) continue;
                var color = tile.TileType == TileType.GROUND ? Color.White : Color.Orange;
                
                GraphicsUtils.Instance.SpriteBatch.Draw(
                    GraphicsUtils.Instance.PixelTexture, 
                    new Rectangle(tile.X * 4, tile.Y * 4,
                        4, 4), 
                    color);
            }
            
            foreach (var player in Players.Select(playerKeyValuePair => playerKeyValuePair.Value))
            {
                var name = player.SteamId == (uint) SteamClient.SteamId ? 
                    SteamFriends.GetPersona() : 
                    SteamFriends.GetFriendPersona(player.SteamId);
                
                var text = $"{name}";
                var size = GraphicsUtils.Instance.DebugFont.MeasureString(text);
                
                GraphicsUtils.Instance.SpriteBatch.DrawString(
                    GraphicsUtils.Instance.DebugFont, $"{text}", new Vector2(player.X + 5 - size.X / 2.0f, player.Y - size.Y - 5), Color.Aqua);
                GraphicsUtils.Instance.DrawRectangle(player.X, player.Y, 10, 10, Color.Aqua);
            }
            
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
            
        }

        public void OnNewConnection(Connection connection, ConnectionInfo info) => 
            Players.Add((uint) info.Identity.SteamId, new Player()
            {
                SteamId = (uint) SteamClient.SteamId
            });

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
                player.X = movePacket.X;
                player.Y = movePacket.Y;
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
                    player.X = movePacket.X;
                    player.Y = movePacket.Y;
                }
                else
                {
                    var player = new Player()
                    {
                        SteamId = movePacket.SteamId,
                        X = movePacket.X,
                        Y = movePacket.Y
                    };
                    
                    Players.Add(player.SteamId, player);
                }
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