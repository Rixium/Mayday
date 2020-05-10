namespace Yetiface.Engine.Graphics
{
    public interface IAnimation
    {
        void Initialize(string filePath);
        void Update();
        void Draw();
    }
}