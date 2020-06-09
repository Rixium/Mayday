using System;

namespace Yetiface.Engine.Utils
{
    public static class Randomizer
    {
        
        public static Random Random { get; set; } = new Random();

        public static void ReSeed(int seed) => 
            Random = new Random(seed);

        public static int Next(int min, int max) =>
            Random.Next(min, max);
        
        public static double NextDouble(double min, double max) =>
            Random.NextDouble() * (max - min) + min;

    }
}