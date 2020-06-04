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
            
            _packager = new MaydayMessagePackager();
        }

        public void Draw()
        {
            
        }

        public void Update()
        {
            
        }

        public void AfterDraw()
        {
            
        }
    }
}