﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Screens.Transitions;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class YetiGame : Game
    {
        
        public string GameName = "YetiGame";
        public string GameVersion => $"{Major}.{Minor}.{Revision}";
        public double UpdateTimeAccumulator { get; set; }

        protected int Major = 0;
        protected int Minor = 1;
        protected int Revision = 0;
        
        public static ContentManager ContentManager;
        public static IInputManager InputManager;
        public IScreenManager ScreenManager { get; set; }

        private readonly FrameRate _frameRate;

        public YetiGame(string gameName)
        {
            GameName = gameName;

            Utils.Window.GraphicsDeviceManager = Utils.Window.CreateGraphicsDevice(this);

            Content.RootDirectory = "Content";
            ScreenManager = new ScreenManager();
            ContentManager = Content;
            
            _frameRate = new FrameRate();
        }

        protected override void Initialize()
        {
            IsMouseVisible = false;
            
            SetWindowTitle();

            InputManager = new InputManager();
            InputManager.Initialize(File.ReadAllText("Config/inputBindings.json"));

            base.Initialize();
        }

        private void UnlockFps()
        {
            Utils.Window.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        private void SetWindowTitle(params string[] supplementaryText)
        {
            Window.Title = $"{GameName} - Version {GameVersion} - {string.Join(" - ", supplementaryText)}";
        }

        protected override void LoadContent()
        {
            GraphicsUtils.Instance.SpriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsUtils.Instance.Load(ContentManager);
            
            ScreenManager.SetScreenTransition(
                new FadeTransition
                {
                    Speed = 3f
                }
            );
            
            ScreenManager.AddScreen(
                new SplashScreen
                {
                    BackgroundColor = Color.White
                }
             );

            InputManager.RegisterInputEvent(new KeyInputBinding(Keys.F1),
                callback: () =>
                {
                    var activeScreen = ScreenManager.GetActiveScreen();
                    activeScreen.IsDebug = !activeScreen.IsDebug;
                });
        }

        protected override void Update(GameTime gameTime)
        {
            // if (!ShouldUpdate(ref gameTime))
            //     return;

            if(ShouldQuit) Exit();
            
            // Update all of our util stuff !DO FIRST ALWAYS
            UtilManager.Update(gameTime);
            
            InputManager.Update();
            ScreenManager.Update();
            
            UtilManager.AfterUpdate(gameTime);
            
            // base update. Calls the base classes update method !DO LAST ALWAYS
            base.Update(gameTime);
        }

        private bool ShouldUpdate(ref GameTime gameTime)
        {            
            UpdateTimeAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
            
            if (UpdateTimeAccumulator < 1 / 60f)
            {
                return false;
            }
            gameTime = new GameTime(gameTime.TotalGameTime, new TimeSpan(10000000 / 60));
            UpdateTimeAccumulator -= 1 / 60f;
            UpdateTimeAccumulator = Math.Min(UpdateTimeAccumulator, 1 / 60f);

            return true;
        }

        protected override void Draw(GameTime gameTime)
        {
            _frameRate.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            SetWindowTitle($"{Math.Round(_frameRate.AverageFramesPerSecond)} fps");
            ScreenManager.Draw();
            base.Draw(gameTime);
        }

        public static void Quit()
        {
            ShouldQuit = true;
        }

        private static bool ShouldQuit { get; set; }
    }
}
