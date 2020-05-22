namespace Yetiface.Engine.UI
{
    public interface IUserInterface
    {
        IElement Root { get; }

        void Update();

        void Draw();

        void DrawDebug();

        /// <summary>
        /// Adds an element to the root of the user interface.
        /// </summary>
        T AddElement<T>(T element) where T : IElement;
    }
}