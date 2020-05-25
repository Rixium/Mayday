﻿using Mayday.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.Networking.SteamNetworking;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
            BackgroundColor = Color.Black;

            // TODO for now this is here, but it should be moved to somewhere else
            // like some kind of network setup manager or SOMETHING.
            // otherwise this has to be passed between screens.
            var networkManager = new SteamNetworkManager(Game1.AppId);
            
            var menuScreenUserInterface = new MenuScreenUserInterface(networkManager);
            
            UserInterface = menuScreenUserInterface;
            
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Draw()
        {
            base.Draw();
            
            GraphicsUtils.Instance.Begin(false);
            var args = System.Environment.GetCommandLineArgs();

            int num = 0;
            foreach (var param in args)
            {
                num++;
                GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont, param, new Vector2(10, num * 50), Color.Green);
            }
            
            GraphicsUtils.Instance.End();
        }

        public override void Finish()
        {
        }
    }
}