using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI.Widgets
{
    public class Button : Element
    {
        public Element TextElement;
        
        public Button(string text = null, int offsetX = 0, int offsetY = 0) : base(offsetX, offsetY)
        {
            FillColor = Color.Black * 0.7f;
            FillToParent = false;
            Height = 60;
            Width = 175;
            if (text != null)
            {
                TextElement = this.AddElement(new TextBlock(text));
            }
        }

        public override void DrawElement()
        {
            
        }
    }
}