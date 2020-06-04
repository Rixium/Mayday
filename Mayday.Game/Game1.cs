﻿using System;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Myra.Graphics2D.UI.Styles;
using Steamworks;
using XNAssets;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;

namespace Mayday.Game
{
    public class Game1 : YetiGame
    {

        public Game1() : base("Mayday")
        {
            try 
            {
                SteamClient.Init(AppId);
            }
            catch ( Exception)
            {
                // Couldn't init for some reason (steam is closed etc)
            }
        }
        
        public static uint AppId { get; set; } = 1323490;

        public static int FriendCount { get; set; }

        public static string Name { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
            
            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F1), NextResize);
        }

        protected override void Update(GameTime gameTime)
        {
            SteamClient.RunCallbacks();
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            var music  = ContentManager.Load<Song>("MainMenu/menuMusic");
            MediaPlayer.Play(music);
            MediaPlayer.Volume = 0.1f;

            new ContentChest().Load(ContentManager);
            
            Myra.MyraEnvironment.Game = this;
            
            ScreenManager.AddScreen(new MenuScreen());
        }

        public static void NextResize()
        {
            switch (Yetiface.Engine.Utils.Window.WindowWidth)
            {
                case 640:
                    Yetiface.Engine.Utils.Window.ResizeWindow(1280, 720);
                    break;
                case 1280:
                    Yetiface.Engine.Utils.Window.ResizeWindow(1920, 1080);
                    break;
                case 1920:
                    Yetiface.Engine.Utils.Window.ResizeWindow(640, 360);
                    break;
            }
        }
    }
    
}
