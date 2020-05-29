using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Assets;

namespace Mayday.Game.Assets
{
    public class MaydayAssetManager : AssetManager
    {
        
        public AssetChest<Texture2D> Tiles { get; set; }
        
        public AssetChest<Texture2D> Heads { get; set; }
        public AssetChest<Texture2D> Torsos { get; set; }
        public AssetChest<Texture2D> Arms { get; set; }
        public AssetChest<Texture2D> Legs { get; set; }

    }
}