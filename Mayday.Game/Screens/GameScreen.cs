using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Mayday.Game.Gameplay;
using Mayday.Game.Networking;
using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Steamworks.Data;
using Yetiface.Engine;
using Yetiface.Engine.Networking;
using Yetiface.Engine.Networking.SteamNetworking;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using MouseState = Yetiface.Engine.Utils.MouseState;
using Rectangle = System.Drawing.Rectangle;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen, INetworkServerListener, INetworkClientListener
    {
        private readonly INetworkManager _networkManager;

        private IWorld _world;

        private Texture2D _texture;
        public Bitmap Bitmap;
        private Tile _lastTile;
        private int _sendTile;
        private INetworkMessageParser _messageParser;

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
            _texture = GetTexture(GraphicsUtils.Instance.SpriteBatch.GraphicsDevice, Bitmap);
        }
        
        private Texture2D GetTexture(GraphicsDevice dev, Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(dev, bmp.Width, bmp.Height);

            unsafe
            {
                // lock bitmap
                BitmapData origdata = 
                    bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                uint* byteData = (uint*)origdata.Scan0;

                // Switch bgra -> rgba
                for (int i = 0; i < imgData.Length; i++)
                {
                    byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);                        
                }                

                // copy data
                Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                // unlock bitmap
                bmp.UnlockBits(origdata);
            }

            texture.SetData(imgData);

            return texture;
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
                    new Microsoft.Xna.Framework.Rectangle(tile.X * 4, tile.Y * 4,
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
                    _sendTile = 0;
                    SendMap();
                    break;
                case MessageType.WorldSendComplete:
                    break;
                case MessageType.TileData:
                    var tileData = (TileData) result;
                    _world.Tiles[tileData.X, tileData.Y].TileType = tileData.TileType;
                    break;
                case MessageType.TileReceived:
                    _sendTile++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void SendMap()
        {
            await Task.Delay(1);

            Tile lastTile = null;

            for (var i = 0; i < 200; i++)
            {
                for (var j = 0; j < 200; j++)
                {
                    var currentTile = _world.Tiles[i, j];
                    SendTile(currentTile);
                    _sendTile = 0;
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