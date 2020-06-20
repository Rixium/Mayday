using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.World
{
    public class GameWorld : IGameWorld
    {
        private static ulong _worldObjectEntityId = 1;
        private static ulong CurrentWorldObjectEntityId => _worldObjectEntityId++;

        public int TileSize { get; set; }
        public Tile[,] Tiles { get; set; }

        /// <summary>
        /// The width and height are both in tiles, not pixels.
        /// </summary>
        public int Width { get; set; }

        public int Height { get; set; }
        public IWorldItemSet WorldItems { get; set; } = new WorldItemSet();
        public Action<Tile> TilePlaced { get; set; }
        public HashSet<IEntity> WorldEntities { get; } = new HashSet<IEntity>();

        private HashSet<IEntity> _trackedEntitiesToRemove = new HashSet<IEntity>();

        public IWorldObjectSet WorldObjects { get; set; } = new WorldObjectSet();

        public void Move(IEntity player, float xMove, float yMove, float yVelocity)
        {
            player.X += xMove;

            var bounds = player.GetCurrentBounds();

            var tileStartX = (bounds.Left / TileSize) - 1;
            var tileEndX = (bounds.Right / TileSize) + 1;
            var tileStartY = (bounds.Top / TileSize) - 1;
            var tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = (int) tileStartX; i <= tileEndX; i++)
            {
                for (var j = (int) tileStartY; j <= tileEndY; j++)
                {
                    var tile = TryGetTile(i, j);
                    if (tile == null) continue;
                    if (tile.TileType == TileType.None) continue;

                    var tileBounds = tile.GetCurrentBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var canMoveUp = true;
                    if (j >= tileEndY - 2 && Math.Abs(yVelocity) < 0.01f)
                    {
                        for (var k = j - 1; k > j - 4; k--)
                        {
                            var above = TryGetTile(i, k);

                            if (above.TileType == TileType.None)
                                continue;

                            canMoveUp = false;
                            break;
                        }
                    }
                    else
                    {
                        canMoveUp = false;
                    }

                    if (canMoveUp)
                    {
                        yMove--;
                    }
                    else
                    {
                        var depth = bounds.GetIntersectionDepth(tileBounds);
                        player.X += depth.X;
                        bounds = player.GetCurrentBounds();
                    }
                }
            }

            player.Y += yMove;
            bounds = player.GetCurrentBounds();

            tileStartX = (bounds.Left / TileSize) - 1;
            tileEndX = (bounds.Right / TileSize) + 1;
            tileStartY = (bounds.Top / TileSize) - 1;
            tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = (int) tileStartX; i <= tileEndX; i++)
            {
                for (var j = (int) tileStartY; j <= tileEndY; j++)
                {
                    var tile = TryGetTile(i, j);
                    if (tile == null) continue;
                    if (tile.TileType == TileType.None) continue;

                    var tileBounds = tile.GetCurrentBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var depth = bounds.GetIntersectionDepth(tileBounds);
                    player.Y += depth.Y;
                    bounds = player.GetCurrentBounds();
                }
            }
        }

        public Tile TryGetTile(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0) return null;
            if (tileX > Width - 1 || tileY > Height - 1) return null;
            return Tiles[tileX, tileY];
        }
        
        public void PlaceTile(Tile tile, TileType tileType)
        {
            if (tile.TileType == tileType) return;
            tile.TileType = tileType;
            TilePlaced?.Invoke(tile);
        }

        public Tile GetRandomSpawnLocation() => (from Tile tile in Tiles
                    where tile.TileType == TileType.Dirt
                    select Tiles[(int) (Width / 2.0f), tile.TileY])
                .FirstOrDefault();

        public void DropItem(ItemDrop item)
        {
            item.GameWorld = this;
            WorldItems.Add(item);
            AddTrackedEntity(item);
        }

        public bool AnythingCollidesWith(Tile tile) =>
            WorldEntities.Any(entity => entity != tile &&
                                        tile.GetCurrentBounds().Intersects(entity.GetCurrentBounds()));

        public void AddTrackedEntity(IEntity entity)
        {
            WorldEntities.Add(entity);
            entity.Destroy += OnTrackedEntityDestroyed;
        }

        private void OnTrackedEntityDestroyed(IEntity obj) =>
            _trackedEntitiesToRemove.Add(obj);

        public void PlaceWorldEntity(Tile tile, WorldObjectType worldObjectType)
        {
            var worldObjectData = ContentChest.WorldObjectData[worldObjectType];
            var worldObjectTexture = ContentChest.WorldObjectTextures[worldObjectType];

            var entity = new Entity(CurrentWorldObjectEntityId)
            {
                X = tile.X,
                Y = tile.Y,
                Bounds = worldObjectData.Width == -1 ?
                    new RectangleF(0, 0, worldObjectTexture.Width * Game1.GlobalGameScale, worldObjectTexture.Height * Game1.GlobalGameScale) :
                    new RectangleF(0, 0, worldObjectData.Width, worldObjectData.Height),
                GameWorld = this
            };

            entity.AddComponent(new MoveComponent());
            entity.AddComponent(new GravityComponent());
            entity.AddComponent(new WorldObjectManagerComponent(worldObjectType));
            WorldObjects.Add(entity);
            AddTrackedEntity(entity);
        }

        public void Update()
        {
            if (_trackedEntitiesToRemove.Count <= 0) return;

            foreach (var entity in _trackedEntitiesToRemove)
                WorldEntities.Remove(entity);

            _trackedEntitiesToRemove.Clear();
        }

        public IEntity GetWorldObjectAbove(Tile tile)
        {
            var tileAbove = TryGetTile(tile.TileX, tile.TileY - 1);

            if (tileAbove == null) return null;

            return GetWorldObjectIntersectingTile(tile);

        }

        private IEntity GetWorldObjectIntersectingTile(Tile tile) =>
            WorldObjects.FirstOrDefault(worldObject =>
                worldObject
                    .GetCurrentBounds()
                    .Intersects(tile.GetCurrentBounds())
            );
    }
}