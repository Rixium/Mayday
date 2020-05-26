using System.IO;
using Mayday.Game.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;

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