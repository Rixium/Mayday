namespace Yetiface.Engine.UI
{
    public interface IUserInterface
    {

        void Draw();

        void Update();

        void AfterDraw();
        bool MouseOver { get; set; }
    }
}