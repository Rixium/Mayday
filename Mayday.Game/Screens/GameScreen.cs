using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeonBit.UI;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
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

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {

        private readonly INetworkManager _networkManager;
        private readonly INetworkMessagePackager _messagePackager;
        
        private IGameWorld _gameWorld;
        public Camera Camera = new Camera();
        
        private Dictionary<ulong, Player> Players { get; set; }

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _messagePackager = new NetworkMessagePackager();
            _messagePackager.AddDefinition<TileTypePacket>();
            _messagePackager.AddDefinition<MapRequestPacket>();
            _messagePackager.AddDefinition<PlayerMovePacket>();

            MyPlayer = new Player()
            {
                SteamId = SteamClient.SteamId,
                X = 1000,
                Y = 1000
            };
            
            Players = new Dictionary<ulong, Player> {
            {
                MyPlayer.SteamId, MyPlayer
            }};

            MyPlayer.Animation = "Walk";
            
            BackgroundColor = Color.White;
            Camera.Goto(new Vector2(MyPlayer.X, MyPlayer.Y));
        }

        public Player MyPlayer { get; set; }

        public void SetWorld(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public override void Awake()
        {
            UserInterface = new GameUserInterface();
            
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.D), () => Move(1, 0), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.A), () => Move(-1, 0), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.S), () => Move(0, 1), InputEventType.Held);
            YetiGame.InputManager.RegisterInputEvent(new KeyInputBinding(Keys.W), () => Move(0, -1), InputEventType.Held);
        }

        private void Move(int x, int y)
        {
            var player = Players[SteamClient.SteamId];
            player.X += x;
            player.Y += y;

            var data = new PlayerMovePacket()
            {
                X = player.X,
                Y = player.Y,
                SteamId = SteamClient.SteamId
            };

            var package = _messagePackager.Package(data);
            _networkManager.SendMessage(package);
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

            var head = ContentChest.Heads[MyPlayer.HeadId];
            var body = ContentChest.Bodies[MyPlayer.BodyId];
            var legs = ContentChest.Legs[MyPlayer.LegsId];
            var arms = ContentChest.Arms[MyPlayer.ArmsId];

            var headAnimation = head.Animations["Walk"].Sprite;
            var bodyAnimation = body.Animations["Walk"].Sprite;
            var legsAnimation = legs.Animations["Walk"].Sprite;
            var armsAnimation = arms.Animations["Walk"].Sprite;
            
            
            GraphicsUtils.Instance.SpriteBatch.Draw(
                armsAnimation.Texture, new Vector2(MyPlayer.X, MyPlayer.Y),
                armsAnimation.SourceRectangle, Color.White, 0, Vector2.Zero, 1,
                SpriteEffects.FlipHorizontally, 0F);
            
            GraphicsUtils.Instance.SpriteBatch.Draw(
                headAnimation.Texture, new Vector2(MyPlayer.X, MyPlayer.Y),
                headAnimation.SourceRectangle, Color.White);
            GraphicsUtils.Instance.SpriteBatch.Draw(
                bodyAnimation.Texture, new Vector2(MyPlayer.X, MyPlayer.Y),
                bodyAnimation.SourceRectangle, Color.White);
            GraphicsUtils.Instance.SpriteBatch.Draw(
                legsAnimation.Texture, new Vector2(MyPlayer.X, MyPlayer.Y),
                legsAnimation.SourceRectangle, Color.White);
            GraphicsUtils.Instance.SpriteBatch.Draw(
                armsAnimation.Texture, new Vector2(MyPlayer.X, MyPlayer.Y),
                armsAnimation.SourceRectangle, Color.White);
            
            GraphicsUtils.Instance.End();
            
            UserInterface?.Draw();
        }

        public override void Finish()
        {
        }

        public override void Update()
        {
            MyPlayer.Animation = "";
            
            _networkManager?.Update();
            UserInterface?.Update();
        }

        public void OnNewConnection(Connection connection, ConnectionInfo info) => 
            Players.Add(info.Identity.SteamId, new Player()
            {
                SteamId = info.Identity.SteamId
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
                //SendMap();
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