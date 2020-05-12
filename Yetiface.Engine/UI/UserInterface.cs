namespace Yetiface.Engine.UI
{
    public class UserInterface : IUserInterface
    {
        public IElement Root { get; set; }

        public IElement AddElement(IElement element)
        {
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