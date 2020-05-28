using System;
using System.Drawing;
using System.Threading.Tasks;
using AccidentalNoise;
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
            
            var seed = (uint) DateTime.UtcNow.Ticks;

            var bmp = new Bitmap(WorldWidth, WorldHeight);

            var tiles = new Tile[WorldWidth, WorldHeight];
            var tileNumber = 0;
            var totalTiles = WorldWidth * WorldHeight;
            var ranges = new SMappingRanges();

            ModuleBase combinedTerrain = TerrainPresets.CavesAndMountains(seed);

            double maxVal = 0;
            int scale = 3;

            //finally update our image
            for (int x = 0; x < WorldWidth; x++)
            {
                for (int y = 0; y < WorldHeight; y++)
                {
                    tileNumber++;
                    var percent = (int) ((float) tileNumber / totalTiles * 100);

                    worldGeneratorListener.OnWorldGenerationUpdate(
                        $"Creating World... {percent}%");

                    var p = x / (double) WorldWidth;
                    var q = y / (double) WorldHeight;

                    var nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    var ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    var val = combinedTerrain.Get(nx * scale, ny * scale);
                    var bitmapColor = Color.Black.Lerp(Color.White, val);

                    bmp.SetPixel(x, y, bitmapColor);
                    
                    if(val > 0.5f)
                        tiles[x, y] = new Tile(TileType.GROUND, x, y);
                    else tiles[x, y] = new Tile(TileType.NONE, x, y);
                }
            }

            tileNumber = 0;
            var ores = TerrainPresets.CreateOres(combinedTerrain, seed);

            //finally update our image
            for (int x = 0; x < WorldWidth; x++)
            {
                for (int y = 0; y < WorldHeight; y++)
                {
                    tileNumber++;
                    var percent = (int) ((float) tileNumber / totalTiles * 100);

                    worldGeneratorListener.OnWorldGenerationUpdate(
                        $"Creating Copper... {percent}%");

                    var p = x / (double) WorldWidth;
                    var q = y / (double) WorldHeight;

                    var nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    var ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    var val = ores.Get(nx * scale, ny * scale);
                    if (val > 0.5f)
                    {
                        tiles[x, y] = new Tile(TileType.COPPER, x, y);
                        bmp.SetPixel(x, y, Color.Orange);
                    }
                }
            }
            
            bmp.Save("Map.png");

            worldGeneratorListener.OnWorldGenerationUpdate("Initiating Landing Sequence...");

            world.Tiles = tiles;
            world.Width = WorldWidth;
            world.Height = WorldHeight;
            
            return world;
        }

    }


    public class SMappingRanges
    {
        public double mapx0, mapy0, mapz0, mapx1, mapy1, mapz1;
        public double loopx0, loopy0, loopz0, loopx1, loopy1, loopz1;

        public SMappingRanges()
        {
            mapx0 = mapy0 = mapz0 = loopx0 = loopy0 = loopz0 = -1;
            mapx1 = mapy1 = mapz1 = loopx1 = loopy1 = loopz1 = 1;
        }
    };

    public static class ExtensionMethods
    {
        public static double Lerp(this double start, double end, double amount)
        {
            double difference = end - start;
            double adjusted = difference * amount;
            return start + adjusted;
        }

        public static Color Lerp(this Color colour, Color to, double amount)
        {
            // start colours as lerp-able floats
            double sr = colour.R, sg = colour.G, sb = colour.B;

            // end colours as lerp-able floats
            double er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte) sr.Lerp(er, amount),
                g = (byte) sg.Lerp(eg, amount),
                b = (byte) sb.Lerp(eb, amount);

            // return the new colour
            return Color.FromArgb(r, g, b);
        }
    }

}
