﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Screens;
using Yetiface.Engine.UI;
using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Screens
{
    public class MenuScreen : Screen
    {
        public MenuScreen() : base("MenuScreen")
        {
        }

        public override void Awake()
        {
            var ball = CreateEntity(Window.BottomRight - new Vector2(50, 50));
            ball.AddComponent(new Animation(YetiGame.ContentManager.Load<Texture2D>("Ball"),
                "Content/Assets/Ball.json"));

            ball.Scale = 3;
            
            BackgroundColor = Color.Green;

            var panel = UserInterface.SetRoot(new Panel());
            
            var panel2 = panel.AddElement(new Panel(50, 50)
            {
                FillColor = Color.Black
            });

            panel2.Anchor = Anchor.Center;

            var button = panel2.AddElement(new Button
            {
                Width = 300, 
                Height = 100, 
                FillToParent = false
            });
            
            var text = button.AddElement(new TextBlock("Penis"));
            button.OnClicked += () => ScreenManager.ChangeScreen("Game");
            
            var button2 = panel2.AddElement(new Button
            {
                Width = 300, 
                Height = 100, 
                FillToParent = false
            });
            
            var text2 = button2.AddElement(new TextBlock("I am also penis"));
            
            var button3 = panel2.AddElement(new Button
            {
                Width = 300, 
                Height = 100, 
                FillToParent = false
            });
            
            button3.AddElement(new TextBlock("Well, not me!"));

            var button4 = panel2.AddElement(new Button
            {
                Width = 300, 
                Height = 100, 
                FillToParent = false
            });
            
            button4.AddElement(new TextBlock("Well, not me!"));
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }
    }
}