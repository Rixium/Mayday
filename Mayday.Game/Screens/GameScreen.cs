using System;
using System.Threading.Tasks;
using GeonBit.UI;
using Mayday.Game.Gameplay;
using Mayday.Game.Networking;
using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks.Data;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {
        private readonly INetworkManager _networkManager;
        private INetworkMessageParser _messageParser;

        private IWorld _world;


        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _messageParser = new NetworkMessageParser();
        }

        public void SetWorld(IWorld world)
        {
            _world = world;
        }

        public override void Awake()
        {
            UserInterface = new GameUserInterface();
        }

        public override void Begin()
        {
        }

        public override void Draw()
        {
            GraphicsUtils.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Black);
            GraphicsUtils.Instance.Begin();

            foreach (var tile in _world.Tiles)
            {
                if (tile.TileType == TileType.NONE) continue;
                var color = tile.TileType == TileType.GROUND ? Color.White : Color.Orange;
                
                GraphicsUtils.Instance.SpriteBatch.Draw(
                    GraphicsUtils.Instance.PixelTexture, 
                    new Rectangle(tile.X * 4, tile.Y * 4,
                        4, 4), 
                    color);
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

            if (MouseState.CurrentState.LeftButton == ButtonState.Pressed)
            {
                var bound = MouseState.Bounds;
                try
                {
                    var tile = _world.Tiles[bound.X / 4, bound.Y / 4];

                    var lastType = tile.TileType;
                    tile.TileType = TileType.GROUND;

                    if (tile.TileType != lastType)
                        SendTile(tile);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            
            UserInterface?.Update();
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
            var result = _messageParser.Parse(data, size);

            switch (result.MessageType)
            {
                case MessageType.ChatMessage:
                    break;
                case MessageType.WorldRequest:
                    SendMap();
                    break;
                case MessageType.WorldSendComplete:
                    break;
                case MessageType.TileData:
                    var tileData = (TileData) result;
                    _world.Tiles[tileData.X, tileData.Y].TileType = tileData.TileType;
                    break;
                case MessageType.TileReceived:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void SendMap()
        {
            await Task.Delay(1);
            
            for (var i = 0; i < 200; i++)
            {
                for (var j = 0; j < 200; j++)
                {
                    var currentTile = _world.Tiles[i, j];
                    SendTile(currentTile);
                }
                
                await Task.Delay(1);
            }

            _networkManager.SendMessage(MessageType.WorldSendComplete);
        }

        private void SendTile(Tile tile)
        {
            var toSend = $"{tile.X}:{tile.Y}:{(int)tile.TileType}";
            _networkManager.SendMessage(MessageType.TileData, toSend);
        }

        public void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            
        }


        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {           
            var result = _messageParser.Parse(data, size);

            switch (result.MessageType)
            {
                case MessageType.ChatMessage:
                    break;
                case MessageType.WorldRequest:
                    break;
                case MessageType.WorldSendComplete:
                    break;
                case MessageType.TileData:
                    var tileData = (TileData) result;
                    _world.Tiles[tileData.X, tileData.Y].TileType = tileData.TileType;
                    break;
                case MessageType.TileReceived:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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