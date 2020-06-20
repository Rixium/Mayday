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

        private TileType _tileType = TileType.None;
        
        public TileType TileType
        {
            get => _tileType;
            set
            {
                if (_tileType == value)
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
                    var tile = GameWorld.TryGetTile(i, j);
                    if (tile == null) continue;
                    tile.BlobValue = -1;
                }
            }
        }

        public int WallType { get; set; }

        public Vector2 RenderCenter => new Vector2(X + TileSize / 2.0f, Y + TileSize / 2.0f);
        public int BlobValue { get; set; } = -1;

        public Tile(TileType tileType, int tileX, int tileY) : base(CurrentTileEntityId)
        {
            TileType = tileType;
            TileX = tileX;
            TileY = tileY;
        }

        public override RectangleF GetCurrentBounds() => new RectangleF(X, Y, TileSize, TileSize);

        public void Break()
        {
            if (TileType == TileType.None) return;
            TileType = TileType.None;
            Destroy?.Invoke(this);

            CleanUpComponents();
            
            PacketManager.SendTileChangePacket(this);
        }

    }
}