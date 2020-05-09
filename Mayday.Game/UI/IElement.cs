namespace Mayday.Game.UI
{
    public interface IElement
    {
        
        IUserInterface UserInterface { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        void Update();
        void Draw();
        void DrawDebug();
        
    }
}