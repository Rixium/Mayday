using GeonBit.UI;
using Mayday.Game.Networking.Packagers;
using Mayday.Game.Screens;
using Yetiface.Engine.Networking;
using Yetiface.Engine.UI;
using Yetiface.Engine.Utils;

namespace Mayday.Game.UI
{
    public class GameScreenUserInterface : IUserInterface
    {
        private readonly GameScreen _gameScreen;
        private readonly INetworkManager _netManager;
        private MaydayMessagePackager _packager;
        
        public GameScreenUserInterface(GameScreen gameScreen, INetworkManager netManager)
        {
            _gameScreen = gameScreen;
            _netManager = netManager;
            UserInterface.Active.Clear();
            
            _packager = new MaydayMessagePackager();
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
            UserInterface.Active.DrawMainRenderTarget(GraphicsUtils.Instance.SpriteBatch);
        }

    }
}