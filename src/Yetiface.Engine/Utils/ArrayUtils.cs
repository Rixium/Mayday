namespace Yetiface.Engine.Utils
{
    public static class ArrayUtils
    {
                
        // Convert a 1D array index to a 2D array index.
        public static void IndexConvert(int index, int arrayWidth, out int x, out int y)
        {
            x = index % arrayWidth;
            y = index / arrayWidth;
        }
        
    }
}