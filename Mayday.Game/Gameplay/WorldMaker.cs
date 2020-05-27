using System;
using System.Drawing;
using System.Threading.Tasks;
using AccidentalNoise;
using Yetiface.Engine.Utils;

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

        public Bitmap Bitmap;

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
            await Task.Delay(1);

            var world = new World();

            var worldWidth = 1280 / 2;
            var worldHeight = 720 / 2;

            var seed = (int) DateTime.UtcNow.Ticks;

            var bmp = new Bitmap(worldWidth, worldHeight);

            var tiles = new int[worldWidth, worldHeight];
            var tileNumber = 0;
            var totalTiles = worldWidth * worldWidth;
            var ranges = new SMappingRanges();

            ModuleBase combinedTerrain = TerrainPresets.GetPreset(PresetType.cavesAndMountains);

            double maxVal = 0;

            //finally update our image
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    tileNumber++;
                    var percent = (int) ((float) tileNumber / totalTiles * 100);

                    worldGeneratorListener.OnWorldGenerationUpdate(
                        $"Creating World... {tileNumber}/{totalTiles} tiles");

                    var p = x / (double) worldWidth;
                    var q = y / (double) worldHeight;

                    var nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    var ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    var val = combinedTerrain.Get(nx * 4, ny * 4);
                    var bitmapColor = Color.Black.Lerp(Color.White, val);

                    bmp.SetPixel(x, y, bitmapColor);
                }
            }

            var ores = TerrainPresets.CreateOres(combinedTerrain);

            //finally update our image
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    tileNumber++;
                    var percent = (int) ((float) tileNumber / totalTiles * 100);

                    worldGeneratorListener.OnWorldGenerationUpdate(
                        $"Creating Copper... {tileNumber}/{totalTiles} tiles");

                    var p = x / (double) worldWidth;
                    var q = y / (double) worldHeight;

                    var nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    var ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    var val = ores.Get(nx * 4, ny * 4);
                    if (val > 0.5f)
                        bmp.SetPixel(x, y, Color.Orange);
                }
            }
            
            bmp.Save("Map.png");

            Bitmap = bmp;

            worldGeneratorListener.OnWorldGenerationUpdate("Initiating Landing Sequence...");

            return world;
        }

    }

    public interface IWorldGeneratorListener
    {

        void OnWorldGenerationUpdate(string message);

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
