using GeonBit.UI;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;

namespace Mayday.Game.UI
{
    public class GameScreenUserInterface : IUserInterface
    {
        public GameScreenUserInterface()
        {
            UserInterface.Active.Clear();
            UserInterface.Active.UseRenderTarget = false;
        }
        
        public void Draw()
        {
            UserInterface.Active.Draw(GraphicsUtils.Instance.SpriteBatch);
        }

        public void Update()
        {
            UserInterface.Active.Update(Time.GameTime);
        }

        public void AfterDraw()
        {
            
        }
    }
}