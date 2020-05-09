﻿using Mayday.Game.Utils;
using Microsoft.Xna.Framework;

namespace Mayday.Game.UI
{
    public class Element : IElement
    {
        private bool _debug;
        public IUserInterface UserInterface { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public void Update()
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void DrawDebug()
        {
            GraphicsUtils.Instance.DrawRectangle(X, Y, Width, Height, Color.Red);
        }
    }
}