using Yetiface.Engine.UI.Widgets;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI
{
    public class UserInterface : IUserInterface
    {
        public IElement Root { get; }
        
        public UserInterface()
        {
            Root = new Panel()
            {
                Width = Window.ViewportWidth,
                Height = Window.ViewportHeight,
                UserInterface = this
            };
        }

        public void Update()
        {
            // We make a new hover element and set it to null,
            // now we can pass it as a REFERENCE to the update. I wrote a comment in element
            // to discuss this further.
            IElement hoverElement = null;
            Root?.Update(ref hoverElement);
        }

        public void Draw()
        {
            GraphicsUtils.Instance.Begin(false);
            Root?.Draw();
            GraphicsUtils.Instance.End();
        }

        public void DrawDebug()
        {
            Root?.DrawDebug();
        }

        public T AddElement<T>(T element) where T : IElement
        {
            Root.AddElement(element);
            
            element.UserInterface = this;
            
            return element;
        }
    }
}