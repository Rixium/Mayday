using System.Threading.Tasks;

namespace Mayday.Game.Gameplay
{
    /// <summary>
    /// The world maker is the main type of world maker,
    /// it is just for creating the world through single player or
    /// hosted.
    /// </summary>
    public class WorldMaker : IWorldMaker
    {
        
        public int WorldSize { get; set; }

        public WorldMaker SetWorldSize(int worldSize)
        {
            WorldSize = worldSize;
            return this;
        }
        
        public async Task<IWorld> Create(IWorldGeneratorListener listener)
        {
            var world = await GenerateWorld(listener);
            return world;
        }

        private async Task<IWorld> GenerateWorld(IWorldGeneratorListener worldGeneratorListener)
        {
            var world = new World();
            await Task.Delay(1000);
            worldGeneratorListener.OnWorldGenerationUpdate("Creating the World...");
            await Task.Delay(1000);
            worldGeneratorListener.OnWorldGenerationUpdate("Laying the Foundation...");
            await Task.Delay(1000);
            worldGeneratorListener.OnWorldGenerationUpdate("Building Mountains...");
            await Task.Delay(1000);
            worldGeneratorListener.OnWorldGenerationUpdate("Digging Lakes...");
            await Task.Delay(1000);
            worldGeneratorListener.OnWorldGenerationUpdate("Populating Fauna...");
            await Task.Delay(1000);
            worldGeneratorListener.OnWorldGenerationUpdate("Initiating Landing Sequence...");
            
            return world;
        }
        
    }

    public interface IWorldGeneratorListener
    {

        void OnWorldGenerationUpdate(string message);
        
    }
    
}