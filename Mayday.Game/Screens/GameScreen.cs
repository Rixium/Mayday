using Mayday.Game.Gameplay;
using Yetiface.Engine.Screens;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen
    {
        
        private IWorld _world;

        public GameScreen() : base("GameScreen")
        {
        }

        public void SetWorld(IWorld world)
        {
            _world = world;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Finish()
        {
        }
    }
}