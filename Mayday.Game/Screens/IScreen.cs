using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    public interface IScreen
    {
        IScreenManager ScreenManager { get; set; }
        
        Color BackgroundColor { get; set; }
        
        string Name { get; set; }

        void Initialize();
        
        void Update();

        void Draw();
    }
}