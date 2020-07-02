using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Lighting;

namespace Mayday.Game.Graphics.Renderers
{
    public interface ILightMapRenderer
    {
        void RenderToRenderTarget(LightMap lightMap);

        void Draw(IEntity player, IGameWorld gameWorld);
    }
}