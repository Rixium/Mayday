using Yetiface.Engine.Utils;

namespace Yetiface.Engine.UI
{
    public class UserInterface : IUserInterface
    {
        public IElement Root { get; set; }

        public IElement SetRoot(IElement element)
        {
            element.Width = Window.ViewportWidth;
            element.Height = Window.ViewportHeight;
            element.UserInterface = this;
            Root = element;
            return element;
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
    }
}