using System.Collections.Generic;

namespace Yetiface.Engine.UI
{
    
    public interface IUserInterface
    {

        IElement Root { get; set; }
        
        /// <summary>
        /// Sets the single root object of the userinterface.
        /// </summary>
        /// <param name="element">The element to set as root.</param>
        /// <returns>Returns the actual element that was added again.</returns>
        IElement SetRoot(IElement element);

        void Update();

        void Draw();

        void DrawDebug();
    }
    
}