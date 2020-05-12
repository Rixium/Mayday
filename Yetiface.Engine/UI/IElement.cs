using System.Collections.Generic;

namespace Yetiface.Engine.UI
{
    public interface IElement
    {
        
        IUserInterface UserInterface { get; set; }
        IList<IElement> Children { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        IElement AddElement(IElement element);
        void Update();
        void Draw();
        void DrawDebug();
        
    }
}