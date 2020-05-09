using System.Collections.Generic;

namespace Mayday.Game.UI
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