using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yetiface.Engine.UI
{
    public interface IElement
    {
        
        IUserInterface UserInterface { get; set; }        
        IElement Parent { get; set; }
        IList<IElement> Children { get; set; }
        Vector2 Offset { get; set; }
        /// <summary>
        /// The position relative to the element' parent
        /// </summary>
        Vector2 RelativePosition { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        
        Rectangle RenderRectangle { get; }

        bool FillToParent { get; set; }

        IElement AddElement(IElement element);
        void CalculateRectangle();
        IElement GetPreviousSibling();
        
        void Update();
        void Draw();
        void DrawDebug();
        
    }
}