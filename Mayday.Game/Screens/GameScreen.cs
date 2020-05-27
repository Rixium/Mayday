﻿using System;
using GeonBit.UI;
using Mayday.Game.Gameplay;
using Mayday.Game.Networking;
using Mayday.Game.Networking.PacketDefinitions;
using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks.Data;
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
        
        private readonly INetworkManager _networkManager;
        // ReSharper disable once NotAccessedField.Local
        private readonly INetworkMessageParser _messageParser;
        private readonly INetworkMessagePackager _messagePackager;
        
        private IWorld _world;

        public GameScreen(INetworkManager networkManager) : base("GameScreen")
        {
            _networkManager = networkManager;
            _networkManager.SetServerNetworkListener(this);
            _networkManager.SetClientNetworkListener(this);
            
            _messageParser = new NetworkMessageParser();
            _messagePackager = new NetworkMessagePackager();
            
            _messagePackager.AddDefinition(typeof(TileTypeUpdate), new TileTypePacketDefinition());
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

        /// <summary>
        /// Obviously a test implementation at the moment, so we can see
        /// the world rendering. Tbh ignore everything in this class, as it has just
        /// become a testing ground atm.
        /// </summary>
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
            
        }

        private void SendTile(Tile tile)
        {
            var package = _messagePackager.Package(tile);
            _networkManager.SendMessage(package);
        }

        public void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            
        }


        public void OnDisconnectedFromServer(ConnectionInfo info)
        {
            
        }

        public void OnMessageReceived(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            
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