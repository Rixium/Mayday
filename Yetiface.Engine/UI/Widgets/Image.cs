﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI.Widgets
{
    /// <summary>
    ///  Fill will stretch the image to the parents size.
    ///  Preserve maintains the image size and just places the image dependant on the parents anchor point, with an origin
    ///  of the images center.
    /// </summary>
    public enum DrawMode
    {
        Stretch,
        Preserve
    }

    public class Image : Element
    {
        private readonly Texture2D _texture;
        private readonly DrawMode _drawMode;

        public Image(Texture2D texture, DrawMode drawMode = DrawMode.Preserve)
        {
            _texture = texture;
            _drawMode = drawMode;

            Width = texture.Width;
            Height = texture.Height;
        }

        public float Rotation { get; set; }

        public override void DrawElement()
        {
            switch (_drawMode)
            {
                case DrawMode.Stretch:
                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        _texture,
                        RenderRectangle, Color.White);
                    break;
                case DrawMode.Preserve:
                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        _texture,
                        new Vector2(RenderRectangle.X + Width / 2.0f, RenderRectangle.Y + Height / 2.0f),
                        rotation: Rotation,
                        origin: new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f),
                        color: Color.White);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}