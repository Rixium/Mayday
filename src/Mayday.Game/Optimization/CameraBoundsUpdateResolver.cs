using Mayday.Game.Gameplay.Entities;
using Yetiface.Engine.Optimization;

namespace Mayday.Game.Optimization
{
    public class CameraBoundsUpdateResolver : IUpdateResolver<IEntity>
    {
        private readonly Camera _camera;

        public CameraBoundsUpdateResolver(Camera camera)
        {
            _camera = camera;
        }
        
        public bool ShouldUpdate(IEntity entity) => _camera.Intersects(entity.GetBounds());
        
    }
}