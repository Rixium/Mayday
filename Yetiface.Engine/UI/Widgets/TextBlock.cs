using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            DrawOutline();
            
            var textSize = GraphicsUtils.Instance.DebugFont.MeasureString(Text);

            var newX = RenderRectangle.X + Parent.RenderRectangle.Width / 2.0f - textSize.X / 2.0f;
            var newY = RenderRectangle.Y + Parent.RenderRectangle.Height / 2.0f - textSize.Y / 2.0f;

            var position = new Vector2(newX, newY);
            var origin = Vector2.Zero;
            var scale = 1f;
            
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont, Text, position, Color,
                0, origin, scale, SpriteEffects.None, 1);
        }

        private void DrawOutline()
        {
            var textSize = GraphicsUtils.Instance.DebugFont.MeasureString(Text);
            var newX = RenderRectangle.X + Parent.RenderRectangle.Width / 2.0f - textSize.X / 2.0f;
            var newY = RenderRectangle.Y + Parent.RenderRectangle.Height / 2.0f - textSize.Y / 2.0f;
            var position = new Vector2(newX, newY);
            var outlineWidth = 2;
            var outlineColor = Color.Black;
            var origin = Vector2.Zero;
            var scale = 1f;
            
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont, Text, position + Vector2.UnitX * outlineWidth, outlineColor,
                0, origin, scale, SpriteEffects.None, 0.5f);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont, Text, position - Vector2.UnitX * outlineWidth, outlineColor,
                0, origin, scale, SpriteEffects.None, 0.5f);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont, Text, position + Vector2.UnitY * outlineWidth, outlineColor,
                0, origin, scale, SpriteEffects.None, 0.5f);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont, Text, position - Vector2.UnitY * outlineWidth, outlineColor,
                0, origin, scale, SpriteEffects.None, 0.5f);
        }
    }
}