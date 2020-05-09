using System.Collections.Generic;

namespace Mayday.Game.UI
{
    
    public interface IUserInterface
    {

        IList<IElement> Element { get; set; }

        void Update();

        void Draw();

    }
    
}