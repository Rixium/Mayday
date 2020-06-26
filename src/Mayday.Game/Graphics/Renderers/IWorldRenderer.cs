using Mayday.Game.Gameplay.World.Areas;

namespace Mayday.Game.Graphics.Renderers
{
    public interface IWorldRenderer
    {
        void Draw(IGameArea gameWorld, Camera camera);
    }
}