using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Lighting;

namespace Mayday.Game.Graphics.Renderers
{
    public interface IGameRenderer
    {

        void Draw(Camera camera,
            IGameWorld gameWorld,
            IEntitySet entitySet,
            IEntity myPlayer,
            LightMap lightMap);
    }
}