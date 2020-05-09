namespace Mayday.Game.Graphics
{
    public interface IAnimation
    {
        void Initialize(string filePath);
        void Update();
        void Draw();
    }
}