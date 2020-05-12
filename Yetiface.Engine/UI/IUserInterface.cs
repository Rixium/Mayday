using System.Collections.Generic;

namespace Yetiface.Engine.UI
{
    
    public interface IUserInterface
    {

        IElement Root { get; set; }
        
        IElement AddElement(IElement element);

        void Update();

        void Draw();

        void DrawDebug();
    }
    
}