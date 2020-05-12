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

        public override void Draw()
        {
            base.Draw();
            GraphicsUtils.Instance.SpriteBatch.DrawString(
                GraphicsUtils.Instance.DebugFont, Text, new Vector2(RenderRectangle.X, RenderRectangle.Y), Color.White);
        }
    }
}