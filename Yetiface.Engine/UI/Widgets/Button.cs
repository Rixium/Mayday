using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI.Widgets
{
    public class Button : Element
    {
        public Element TextElement;
        
        public Button(string text = null)
        {
            FillColor = Color.Black * 0.7f;
            Height = 60;
            Width = 175;
            
            if (text != null)
            {
                TextElement = AddElement(new TextBlock(text)
                {
                    CanInteractWithThisPieceOfShit = false
                });
            }
        }

        public override void DrawElement()
        {
            
        }
    }
}