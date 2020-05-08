using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    public interface IScreen
    {

        Color BackgroundColor { get; set; }
        
        string Name { get; set; }
        
        void Update();

        void Draw();
        
    }
}