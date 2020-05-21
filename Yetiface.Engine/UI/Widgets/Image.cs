using System;
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
        Fill,
        Preserve
    }
    
    public class Image : Element
    {
        private readonly Texture2D _texture;
        private readonly DrawMode DrawMode;

        public Image(Texture2D texture, DrawMode drawMode = DrawMode.Fill, int offsetX = 0, int offsetY = 0, bool fillToParent = true) : base(offsetX, offsetY,
            fillToParent)
        {
            _texture = texture;
            DrawMode = drawMode;

            Width = texture.Width;
            Height = texture.Height;
        }


        public override void DrawElement()
        {
            switch (DrawMode)
            {
                case DrawMode.Fill:
                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        _texture,
                        RenderRectangle, Color.White);
                    break;
                case DrawMode.Preserve:
                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        _texture,
                        new Vector2(RenderRectangle.X, RenderRectangle.Y),
                        null,
                        Color.White,
                        0.0f,
                        new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f), 
                        1, 
                        SpriteEffects.None, 
                        0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}