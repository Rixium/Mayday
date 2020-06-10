using Mayday.Game.Gameplay.World;

namespace Mayday.Game.Graphics.Renderers
{
    public interface IWorldRenderer
    {
        void Draw(IGameWorld gameWorld, Camera camera);
    }
}