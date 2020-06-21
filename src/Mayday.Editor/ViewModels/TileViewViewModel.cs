using Mayday.Game.Gameplay.Data;

namespace Mayday.Editor.ViewModels
{
    public class TileViewViewModel
    {
        public TileProperties Tile { get; set; }

        public string Key { get; set; }

        public TileViewViewModel(string key, TileProperties tile)
        {
            Key = key;
            Tile = tile;
        }

    }
}