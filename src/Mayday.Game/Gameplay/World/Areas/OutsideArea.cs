using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.World.Areas
{
    public class OutsideArea : IGameArea
    {

        private static ulong _gameAreaEntityId = 1;
        private static ulong CurrentGameAreaEntityId => _gameAreaEntityId++;

        public IGameWorld GameWorld { get; set; }
        private int TileSize => GameWorld.TileSize;
        public Tile[,] Tiles { get; set; }
        public int AreaWidth { get; set; }
        public int AreaHeight { get; set; }

        public IWorldItemSet WorldItems { get; set; } = new WorldItemSet();
        public IWorldObjectSet WorldObjects { get; set; } = new WorldObjectSet();

        public void Move(IEntity entity, float xMove, float yMove, float yVelocity)
        {
            entity.X += xMove;

            var bounds = entity.GetCurrentBounds();

            var tileStartX = (bounds.Left / TileSize) - 1;
            var tileEndX = (bounds.Right / TileSize) + 1;
            var tileStartY = (bounds.Top / TileSize) - 1;
            var tileEndY = (bounds.Bottom / TileSize) + 1;

            for (var i = (int) tileStartX; i <= tileEndX; i++)
            {
                for (var j = (int) tileStartY; j <= tileEndY; j++)
                {
                    var tile = TryGetTile(i, j);

                    if (tile == null)
                    {
                        tile = new Tile(TileTypes.Dirt, i, j)
                        {
                            GameWorld = entity.GameWorld,
                            GameArea = entity.GameArea
                        };
                    }

                    if (tile.TileType == TileTypes.None) continue;

                    var tileBounds= tile.GetCurrentBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var depth = bounds.GetIntersectionDepth(tileBounds);
                    entity.X += depth.X;
                    bounds = entity.GetCurrentBounds();
                }
            }

            entity.Y += yMove;
            bounds = entity.GetCurrentBounds();

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
                    if (tile.TileType == TileTypes.None) continue;

                    var tileBounds = tile.GetCurrentBounds();

                    if (!bounds.Intersects(tileBounds)) continue;

                    var depth = bounds.GetIntersectionDepth(tileBounds);
                    entity.Y += depth.Y;
                    bounds = entity.GetCurrentBounds();
                }
            }
        }

        public Tile TryGetTile(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0) return null;
            if (tileX > AreaWidth - 1 || tileY > AreaHeight - 1) return null;
            return Tiles[tileX, tileY];
        }

        public void DropItem(ItemDrop item)
        {
            item.GameWorld = GameWorld;
            item.GameArea = this;
            WorldItems.Add(item);
            GameWorld.AddTrackedEntity(item);
        }

        public void PlaceWorldEntity(Tile tile, string worldObjectType)
        {
            var worldObjectData = ContentChest.WorldObjectData[worldObjectType];
            var worldObjectTexture = ContentChest.WorldObjectTextures[worldObjectType];

            var entity = new Entity(CurrentGameAreaEntityId)
            {
                X = tile.X,
                Y = tile.Y,
                Bounds = worldObjectData.Width == -1 ?
                    new RectangleF(0, 0, worldObjectTexture.Width, worldObjectTexture.Height) :
                    new RectangleF(0, 0, worldObjectData.Width, worldObjectData.Height),
                GameWorld = GameWorld,
                GameArea = this
            };

            entity.AddComponent(new MoveComponent());
            entity.AddComponent(new GravityComponent());
            entity.AddComponent(new WorldObjectManagerComponent(worldObjectType));

            if (worldObjectData.CanBeUsed)
            {
                var worldUseComponent = new UsableWorldObjectComponent(GameWorld.RequestClientPlayer?.Invoke(), worldObjectData);
                worldUseComponent.InRangeOfWorldObject += GameWorld.PlayerInRangeOfWorldObject;
                worldUseComponent.LeftRangeOfWorldObject += GameWorld.PlayerLeftRangeOfWorldObject;
                entity.AddComponent(worldUseComponent);
            }

            WorldObjects.Add(entity);
            GameWorld.AddTrackedEntity(entity);
        }

        public IEntity GetWorldObjectAbove(Tile tile)
        {
            var tileAbove = TryGetTile(tile.TileX, tile.TileY - 1);
            return tileAbove == null ? null : GetWorldObjectIntersectingTile(tile);
        }

        private IEntity GetWorldObjectIntersectingTile(Tile tile) =>
            WorldObjects.FirstOrDefault(worldObject =>
                worldObject
                    .GetCurrentBounds()
                    .Intersects(tile.GetCurrentBounds())
            );

        public Tile GetRandomSpawnLocation() => (from Tile tile in Tiles
                where tile.TileType == TileTypes.Dirt
                select Tiles[(int) (AreaWidth / 2.0f), tile.TileY])
            .FirstOrDefault();

        public IEnumerable<IEntity> GetItems() =>
            WorldItems.Items;

        public void PlaceTile(Tile tile, string tileType)
        {
            if (tile.TileType == tileType) return;
            tile.TileType = tileType;
            GameWorld.TilePlaced?.Invoke(tile);
        }
    }
}