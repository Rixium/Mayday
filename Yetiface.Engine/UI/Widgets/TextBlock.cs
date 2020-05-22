using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI.Widgets
{
    public class TextBlock : Element
    {
        public string Text { get; set; }
        public Color Color { get; set; } = Color.White;

        public TextBlock(string text)
        {
            Text = text;
        }

        public override void DrawElement()
        {
            var textSize = GraphicsUtils.Instance.DebugFont.MeasureString(Text);

            var newX = RenderRectangle.X + Parent.RenderRectangle.Width / 2.0f - textSize.X / 2.0f;
            var newY = RenderRectangle.Y + Parent.RenderRectangle.Height / 2.0f - textSize.Y / 2.0f;

            GraphicsUtils.Instance.SpriteBatch.DrawString(
                GraphicsUtils.Instance.DebugFont, Text, new Vector2(newX, newY), Color);
        }
    }
}