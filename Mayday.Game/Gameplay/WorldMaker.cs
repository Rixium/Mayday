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

            var groundGradient = new Gradient(0, 0, 0, 1);
            
            var lowlandShapeFractal = new Fractal(FractalType.BILLOW, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 2, 0.25f,null);
            var lowlandAutoCorrect = new AutoCorrect(lowlandShapeFractal, 0, 1);
            var lowlandScale = new ScaleOffset(  0.125f, 0.45f, lowlandAutoCorrect);
            var lowlandYScale = new ScaleDomain(lowlandScale, null, 0);
            var lowlandTerrain = new TranslatedDomain(groundGradient,null, lowlandYScale);

            var highlandShapeFractal =
                new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 4, 2, null);
            var highlandAutoCorrect = new AutoCorrect(highlandShapeFractal, -1, 1);
            var highlandScale = new ScaleOffset(0.25f, 0, highlandAutoCorrect);
            var highlandYScale = new ScaleDomain(highlandScale, null, 0);
            var highlandTerrain = new TranslatedDomain(groundGradient, null, highlandYScale);
            
            var mountainShapeFractal = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 8, 1, null);
            var mountainAutoCorrect = new AutoCorrect(mountainShapeFractal, -1, 1);
            var mountainScale = new ScaleOffset(0.45f, 0.15f, mountainAutoCorrect);
            var mountainYScale = new ScaleDomain(mountainScale, null, 0.25f);
            var mountainTerrain = new TranslatedDomain(groundGradient, null, mountainYScale);
            
            var terrainTypeFractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 3, 0.125f, null);
            var terrainAutoCorrect = new AutoCorrect(terrainTypeFractal, 0, 1);
            var terrainTypeYScale = new ScaleDomain(terrainAutoCorrect, null, 0);
            var terrainTypeCache = new Cache(terrainTypeYScale);
            
            var highlandMountainSelect = new Select(terrainTypeCache, highlandTerrain, mountainTerrain , 0.55f, 0.2f);
            var highlandLowlandSelect = new Select(terrainTypeCache, lowlandTerrain, highlandMountainSelect, 0.25f, 0.15f);
            var highlandLowlandSelectCache = new Cache(highlandLowlandSelect);
            
            var groundSelect = new Select(highlandLowlandSelectCache, 0, 1, 0.5f, 0);
            
            var caveShape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 4, null);
            var caveAttenuateBias = new Bias(highlandLowlandSelectCache, 0.45f);
            var caveShapeAttenuate = new Combiner(CombinerTypes.MULT, caveShape, caveAttenuateBias);
            var cavePerturbFractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, null);
            var cavePerturbScale = new ScaleOffset(0.5f, 0, cavePerturbFractal);
            var cavePerturb = new TranslatedDomain(caveShapeAttenuate, cavePerturbScale, null);
            var caveSelect = new Select(cavePerturb, 1, 0, 0.48f, 0);
            var groundCaveMultiply = new Combiner(CombinerTypes.MULT, caveSelect, groundSelect);

            var ranges = new SMappingRanges();
            
            var tiles = new int[Window.ViewportWidth, Window.ViewportHeight];
            
            worldGeneratorListener.OnWorldGenerationUpdate("Forming World... 0%");

            var tileNumber = 0;
            var bmp = new Bitmap(Window.ViewportWidth, Window.ViewportHeight);
            
            var tempColor = new Microsoft.Xna.Framework.Color(0, 0, 0);

            var totalTiles = Window.ViewportWidth * Window.ViewportHeight;
            
            //finally update our image
            for (int x = 0; x < Window.ViewportWidth; x++)
            {
                for(int y = 0; y < Window.ViewportHeight; y++)
                {
                    tileNumber++;
                    var percent = (int)((float)tileNumber / totalTiles * 100);
                    
                    worldGeneratorListener.OnWorldGenerationUpdate($"Forming World... {percent}%");
                    
                    var p = x / (double)Window.ViewportWidth;
                    var q = y / (double)Window.ViewportHeight;

                    var nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    var ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    var caveVal = (int) groundCaveMultiply.Get(nx * 6, ny * 3);
                    
                    tiles[x, y] = caveVal;
                    
                    var bitmapColor = Color.Black.Lerp(Color.White, caveVal);

                    tempColor.R = bitmapColor.R;
                    tempColor.G = bitmapColor.G;
                    tempColor.B = bitmapColor.B;
                    
                    bmp.SetPixel(x, y, bitmapColor);
                }
            }

            bmp.Save("Map.png");

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
        public double mapx0,mapy0,mapz0, mapx1,mapy1,mapz1;
        public double loopx0,loopy0,loopz0, loopx1,loopy1,loopz1;

        public SMappingRanges()
        {
            mapx0=mapy0=mapz0=loopx0=loopy0=loopz0=-1;
            mapx1=mapy1=mapz1=loopx1=loopy1=loopz1=1;
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
            byte r = (byte)sr.Lerp(er, amount),
                g = (byte)sg.Lerp(eg, amount),
                b = (byte)sb.Lerp(eb, amount);

            // return the new colour
            return Color.FromArgb(r, g, b);
        }
    }
    
}
