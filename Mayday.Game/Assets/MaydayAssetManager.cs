using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Assets;

namespace Mayday.Game.Assets
{
    public class MaydayAssetManager : AssetManager
    {
        
        public static AssetChest<Texture2D> Tiles { get; set; }
        
        public static AssetChest<Texture2D> Heads { get; set; }
        public static AssetChest<Texture2D> Torsos { get; set; }
        public static AssetChest<Texture2D> Arms { get; set; }
        public static AssetChest<Texture2D> Legs { get; set; }

    }
}