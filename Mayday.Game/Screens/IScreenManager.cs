using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens
{
    public interface IScreenManager
    {
        
        void AddScreen(IScreen screen);

        void RemoveScreen(IScreen screen);

        void ChangeScreen(string screenName);

        void Update(float delta);

        void Draw(SpriteBatch spriteBatch);

    }
}