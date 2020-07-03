using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.World.Areas;
using Mayday.Game.Lighting;

namespace Mayday.Game.Graphics.Renderers
{
    public interface ILightMapRenderer
    {
        void RenderToRenderTarget(Camera camera, IGameArea gameArea, LightMap lightMap);

        void Draw(Camera camera, IEntity player, IGameWorld gameWorld);
    }
}