using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;

namespace Mayday.Game.Gameplay.Items.Placers
{
    public interface IPlacer
    {
        bool PlacerFor(IItem item);
        bool Place(IEntity entity, IGameWorld gameWorld, IItem selectedItem, int mouseTileX, int mouseTileY);
    }
}