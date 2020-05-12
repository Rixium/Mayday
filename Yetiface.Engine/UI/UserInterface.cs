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
            Root?.Update();
        }

        public void Draw()
        {
            Root?.Draw();
        }

        public void DrawDebug()
        {
            Root?.DrawDebug();
        }
    }
}