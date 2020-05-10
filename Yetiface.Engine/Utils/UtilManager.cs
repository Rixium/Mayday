using Microsoft.Xna.Framework;

namespace Yetiface.Engine.Utils
{
    public static class UtilManager
    {

        /// <summary>
        /// Updates various tools and stuff, so we have easy access to anything like DeltaTime and
        /// the position of the mouse.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            MouseState.Update();
        }
        
    }
    
}