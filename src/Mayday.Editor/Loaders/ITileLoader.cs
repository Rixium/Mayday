using System.Collections.Generic;
using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.Loaders
{
    public interface ITileLoader
    {
        Dictionary<string, TileProperties> Tiles { get; set; }
        void Save();
        void SetTiles(Dictionary<string, TileProperties> tiles);
    }

}