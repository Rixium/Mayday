using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI.Widgets
{
    public class TextBlock : Element
    {
        public string Text { get; set; }

        public TextBlock(string text, int offsetX = 0, int offsetY = 0) : base(offsetX, offsetY)
        {
            Text = text;
        }

        public override void DrawElement()
        {
            var textSize = GraphicsUtils.Instance.DebugFont.MeasureString(Text);

            var newX = RenderRectangle.X + Parent.RenderRectangle.Width / 2.0f - textSize.X / 2.0f;
            var newY = RenderRectangle.Y + Parent.RenderRectangle.Height / 2.0f - textSize.Y / 2.0f;

            GraphicsUtils.Instance.SpriteBatch.DrawString(
                GraphicsUtils.Instance.DebugFont, Text, new Vector2(newX, newY), Color.White);
        }
    }
}