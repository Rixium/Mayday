using System.Collections.Generic;

namespace Yetiface.Engine.UI
{
    
    public interface IUserInterface
    {

        IList<IElement> Elements { get; set; }
        
        void AddElement(IElement element);

        void Update();

        void Draw();

        void DrawDebug();
    }
    
}