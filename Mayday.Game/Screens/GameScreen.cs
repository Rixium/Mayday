using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Mayday.Game.Screens
{

    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {

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

            MyPlayer = new Player
            {
                SteamId = SteamClient.SteamId,
                X = 1000,
                Y = 1000
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

                var package = _messagePackager.Package(data);
                _networkManager.SendMessage(package);
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

            foreach (var player in Players.Select(pair => pair.Value))
            {
                DrawPlayer(player);
            }
            
            GraphicsUtils.Instance.End();
            
            UserInterface?.Draw();
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
        }

        public void OnNewConnection(Connection connection, ConnectionInfo info)
        {
            var newPlayer = new Player
            {
                SteamId = info.Identity.SteamId,
                X = 1000,
                Y = 1000
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
                //SendMap();
            } else if (received.GetType() == typeof(PlayerMovePacket))
            {
                var movePacket = (PlayerMovePacket) received;
                var player = Players[movePacket.SteamId];
                var xDir = movePacket.XDirection;
                player.XDirection = xDir;
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
                    var player = new Player()
                    {
                        SteamId = movePacket.SteamId,
                        X = 1000,
                        Y = 1000,
                        HeadAnimator = new Animator(ContentChest.Heads[MyPlayer.HeadId].Animations),
                        BodyAnimator = new Animator(ContentChest.Bodies[MyPlayer.BodyId].Animations),
                        ArmsAnimator = new Animator(ContentChest.Arms[MyPlayer.ArmsId].Animations),
                        LegsAnimator = new Animator(ContentChest.Legs[MyPlayer.LegsId].Animations)
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