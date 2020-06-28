using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Networking;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.World
{
    public class Tile : Entity
    {

        private static ulong _tileEntityIdCounter = 1;
        private static ulong CurrentTileEntityId => _tileEntityIdCounter++;

        public int TileSize => GameWorld.TileSize;

        private float _x;
        public override float X
        {
            get => TileX * TileSize;
            set => _x = value;
        }

        private float _y;
        public override float Y
        {
            get => TileY * TileSize;
            set => _y = value;
        }

        public int TileX { get; set; }
        public int TileY { get; set; }

        private string _tileType = TileTypes.None;
        
        public string TileType
        {
            get => _tileType;
            set
            {
                if (_tileType == value)
                    return;

                if (value == null)
                    return;

                _tileType = value;
                BlobValue = -1;

                SetNeighboursBlobFlag();
            }
        }

        public TileProperties TileProperties => ContentChest.TileProperties.ContainsKey(TileType)
            ? ContentChest.TileProperties[TileType]
            : null;

        private void SetNeighboursBlobFlag()
        {
            for (var i = TileX - 1; i <= TileX + 1; i++)
            {
                for (var j = TileY - 1; j <= TileY + 1; j++)
                {
                    if (i == TileX && j == TileY) continue;
                    var tile = GameArea.TryGetTile(i, j);
                    if (tile == null) continue;
                    tile.BlobValue = -1;
                }
            }
        }

        public int WallType { get; set; }

        public Vector2 RenderCenter => new Vector2(X + TileSize / 2.0f, Y + TileSize / 2.0f);
        public int BlobValue { get; set; } = -1;

        public Tile(string tileType, int tileX, int tileY) : base(CurrentTileEntityId)
        {
            TileType = tileType;
            TileX = tileX;
            TileY = tileY;
        }

        public override RectangleF GetCurrentBounds() => new RectangleF(X, Y, TileSize, TileSize);

        public void Break()
        {
            if (TileType == TileTypes.None) return;
            TileType = TileTypes.None;
            Destroy?.Invoke(this);

            CleanUpComponents();
            
            PacketManager.SendTileChangePacket(this);
        }

        /// <summary>
        /// For checking whether a tile has an immediate neighbour (North, east, south west).
        /// </summary>
        /// <returns></returns>
        public bool HasImmediateNeighbour()
        {
            var east = GameArea.TryGetTile(TileX - 1, TileY);
            if (east != null && east.TileType != TileTypes.None) return true;
            
            var west = GameArea.TryGetTile(TileX + 1, TileY);
            if (west != null && west.TileType != TileTypes.None) return true;
            
            var north = GameArea.TryGetTile(TileX, TileY - 1);
            if (north != null && north.TileType != TileTypes.None) return true;
            
            var south = GameArea.TryGetTile(TileX, TileY + 1);
            return south != null && south.TileType != TileTypes.None;
        }

        public Tile North => GameArea.TryGetTile(TileX, TileY - 1);
        public Tile NorthEast => GameArea.TryGetTile(TileX + 1, TileY - 1);
        public Tile East => GameArea.TryGetTile(TileX + 1, TileY);
        public Tile SouthEast => GameArea.TryGetTile(TileX + 1, TileY + 1);
        public Tile South => GameArea.TryGetTile(TileX, TileY + 1);
        public Tile SouthWest => GameArea.TryGetTile(TileX - 1, TileY + 1);
        public Tile West => GameArea.TryGetTile(TileX - 1, TileY);
        public Tile NorthWest => GameArea.TryGetTile(TileX - 1, TileY - 1);
        
    }
}