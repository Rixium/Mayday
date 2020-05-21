using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI.Widgets
{
    public class Button : Element
    {
        public Button(int offsetX = 0, int offsetY = 0) : base(offsetX, offsetY)
        {
            FillColor = Color.Black * 0.7f;
            Anchor = Anchor.Center;
        }

        public override void DrawElement()
        {
            
        }
    }
}