using System.Threading.Tasks;

namespace Mayday.Game.Gameplay
{
    /// <summary>
    /// Offer a common interface for all types of ways to make the world.
    /// Be it through network, loaded or manually.
    /// </summary>
    public interface IWorldMaker
    {
        int WorldWidth { get; set; }
        int WorldHeight { get; set; }

        Task<IWorld> Create(IWorldGeneratorListener listener);
    }
}