using System.Threading.Tasks;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.WorldMakers.Listeners;

namespace Mayday.Game.Gameplay.WorldMakers
{
    /// <summary>
    /// Offer a common interface for all types of ways to make the world.
    /// Be it through network, loaded or manually.
    /// </summary>
    public interface IWorldMaker
    {
        int AreaWidth { get; set; }
        int AreaHeight { get; set; }

        Task<IGameWorld> Create(IWorldMakerListener listener);
    }
}