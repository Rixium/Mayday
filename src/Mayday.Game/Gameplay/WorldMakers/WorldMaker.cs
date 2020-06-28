using System.Drawing;
using System.Threading.Tasks;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.World.Areas;
using Mayday.Game.Gameplay.WorldMakers.Listeners;

namespace Mayday.Game.Gameplay.WorldMakers
{
    /// <summary>
    /// The world maker is the main type of world maker,
    /// it is just for creating the world through single player or
    /// hosted.
    /// </summary>
    public class WorldMaker : IWorldMaker
    {
        
        public int AreaWidth { get; set; }
        public int AreaHeight { get; set; }

        public WorldMaker SetWorldSize(int worthWidth, int worldHeight)
        {
            AreaWidth = worthWidth;
            AreaHeight = worldHeight;
            
            return this;
        }

        public async Task<IGameWorld> Create(IWorldMakerListener listener)
        {
            var world = await GenerateWorld(listener);
            return world;
        }

        private async Task<IGameWorld> GenerateWorld(IWorldMakerListener worldGeneratorListener)
        {
            await Task.Delay(1);

            var outsideArea = new OutsideArea();
            var world = new GameWorld(outsideArea)
            {
                TileSize = 32
            };

            var bmp = new Bitmap(AreaWidth, AreaHeight);

            var tiles = new Tile[AreaWidth, AreaHeight];

            for (var i = 0; i < AreaWidth; i++)
            {
                for (var j = 0; j < AreaHeight; j++)
                {
                    tiles[i, j] = new Tile(TileTypes.None, i, j)
                    {
                        GameWorld = world,
                        GameArea = outsideArea
                    };
                }
            }

            var poo = 0;
            for (var i = 0; i < AreaWidth; i++)
            {
                poo = 0;
                for (var j = (int)(AreaHeight / 2.0f); j < AreaHeight; j++)
                {
                    poo++;
                    if (poo > 3)
                    {
                        tiles[i, j].TileType = TileTypes.Stone;
                        bmp.SetPixel(i, j, Color.Gray);
                    }
                    else
                    {
                        tiles[i, j].TileType = TileTypes.Dirt;
                        bmp.SetPixel(i, j, Color.Brown);
                    }

                }
            }
            
            bmp.Save("Map.png");

            worldGeneratorListener.OnWorldGenerationUpdate("Initiating Landing Sequence...");

            outsideArea.Tiles = tiles;
            outsideArea.AreaWidth = AreaWidth;
            outsideArea.AreaHeight = AreaHeight;

            return world;
        }

    }

}
