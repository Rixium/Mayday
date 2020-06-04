namespace Yetiface.Engine.UI
{
    public interface IUserInterface
    {
        void SetActive();

        void Draw();

        void Update();

        void AfterDraw();
    }
}