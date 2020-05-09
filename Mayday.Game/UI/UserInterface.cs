using System.Collections.Generic;

namespace Mayday.Game.UI
{
    public class UserInterface : IUserInterface
    {
        
        public IList<IElement> Elements { get; set; }

        public void AddElement(IElement element)
        {
            if (Elements == null)
                Elements = new List<IElement>();
            
            element.UserInterface = this;
            Elements.Add(element);
        }

        public void Update()
        {
            foreach (var element in Elements)
            {
                element.Update();
            }
        }

        public void Draw()
        {
            foreach (var element in Elements)
            {
                element.Draw();
            }
        }
    }
}