using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public interface IScreen
    {

        string Name { get; set; }
        
        void Update();

        void Draw();
        
    }
}