using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI.Widgets
{
    public class Image : Element
    {
        private readonly Texture2D _texture;

        public Image(Texture2D texture, int offsetX, int offsetY, bool fillToParent = true) : base(offsetX, offsetY,
            fillToParent)
        {
            _texture = texture;

            Width = texture.Width;
            Height = texture.Height;
        }


        public override void DrawElement()
        {
            GraphicsUtils.Instance.SpriteBatch.Draw(
                _texture,
                new Vector2(RenderRectangle.X, RenderRectangle.Y),
                Color.White);
        }
    }
}