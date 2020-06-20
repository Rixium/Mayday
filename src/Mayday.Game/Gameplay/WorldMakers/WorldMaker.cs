using System;
using System.Drawing;
using System.Threading.Tasks;
using AccidentalNoise;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.World;
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
        
        public int WorldWidth { get; set; }
        public int WorldHeight { get; set; }

        public WorldMaker SetWorldSize(int worthWidth, int worldHeight)
        {
            WorldWidth = worthWidth;
            WorldHeight = worldHeight;
            
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

            var world = new GameWorld();
            
            var bmp = new Bitmap(WorldWidth, WorldHeight);

            var tiles = new Tile[WorldWidth, WorldHeight];

            for (var i = 0; i < WorldWidth; i++)
            {
                for (var j = 0; j < WorldHeight; j++)
                {
                    tiles[i, j] = new Tile(TileTypes.None, i, j)
                    {
                        GameWorld = world
                    };
                }
            }

            var poo = 0;
            for (var i = 0; i < WorldWidth; i++)
            {
                poo = 0;
                for (var j = (int)(WorldHeight / 2.0f); j < WorldHeight; j++)
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

            world.Tiles = tiles;
            world.Width = WorldWidth;
            world.Height = WorldHeight;
            world.TileSize = 8 * Game1.GlobalGameScale;
            
            return world;
        }

    }

}
