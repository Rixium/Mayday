using System.Collections.Generic;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.World.Areas;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class WorldRenderer : IWorldRenderer
    {
        private readonly Dictionary<int, int> _tileBlobMap = new Dictionary<int, int>
        {
            {28, 0}, {124, 1}, {112, 2}, {16, 3}, {20, 4}, {116, 5}, {92, 6}, {80, 7}, {84, 8}, {221, 9},
            {31, 11}, {255, 12}, {241, 13}, {17, 14}, {23, 15}, {247, 16}, {223, 17}, {209, 18}, {215, 19}, {119, 20},
            {7, 22}, {199, 23}, {193, 24}, {1, 25}, {29, 26}, {253, 27}, {127, 28}, {113, 29}, {125, 30}, {93, 31},
            {117, 32},
            {4, 33}, {68, 34}, {64, 35}, {0, 36}, {5, 37}, {197, 38}, {71, 39}, {65, 40}, {69, 41}, {87, 42}, {213, 43},
            {21, 48}, {245, 49}, {95, 50}, {81, 51}, {85, 52}
        };

        readonly IList<Tile> _renderedTiles = new List<Tile>();

        public void Draw(IGameArea gameArea, Camera camera)
        {
            var worldTileSize = gameArea.GameWorld.TileSize;

            var startTileX = (int) (camera.Position.X - Window.ViewportWidth / 2.0f) / worldTileSize - 1;
            var startTileY = (int) (camera.Position.Y - Window.ViewportHeight / 2.0f) / worldTileSize - 1;
            var endTileX = (int) (camera.Position.X + Window.ViewportWidth / 2.0f) / worldTileSize + 1;
            var endTileY = (int) (camera.Position.Y + Window.ViewportHeight / 2.0f) / worldTileSize + 1;

            _renderedTiles.Clear();

            for (var i = startTileX; i < endTileX; i++)
            {
                for (var j = startTileY; j < endTileY; j++)
                {
                    if (i < 0 || j < 0 || i > gameArea.AreaWidth - 1 || j > gameArea.AreaHeight - 1) continue;
                    var tile = gameArea.Tiles[i, j];

                    if (tile.TileType == TileTypes.None) continue;
                    var tileSet = ContentChest.TileTextures[tile.TileType];

                    _renderedTiles.Add(tile);

                    var rect = new Rectangle(2, 2, 8, 8);
                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet,
                        new Rectangle((int) tile.X, (int) tile.Y, 8, 8),
                        rect, Color.White);
                }
            }

            // Drawing the edges of tiles
            foreach (var tile in _renderedTiles)
            {
                var tileSet = ContentChest.TileTextures[tile.TileType];

                if (tile.North != null && tile.North.TileType != tile.TileType &&
                    (tile.North.TileProperties?.Name.Length > tile.TileProperties?.Name.Length ||
                     tile.North.TileProperties == null))
                {
                    var rect = new Rectangle(2, 0, 8, 2);
                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet,
                        new Rectangle((int) tile.X, (int) tile.Y - 2, 8, 2),
                        rect, Color.White);
                }

                if (tile.East != null && tile.East.TileType != tile.TileType &&
                    (tile.East.TileProperties?.Name.Length > tile.TileProperties?.Name.Length ||
                     tile.East.TileProperties == null))
                {
                    var rect = new Rectangle(10, 2, 2, 8);
                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet,
                        new Rectangle((int) tile.X + 8, (int) tile.Y, 2, 8),
                        rect, Color.White);
                }

                if (tile.South != null && tile.South.TileType != tile.TileType &&
                    (tile.South.TileProperties?.Name.Length > tile.TileProperties?.Name.Length ||
                     tile.South.TileProperties == null))
                {
                    var rect = new Rectangle(2, 10, 8, 2);
                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet,
                        new Rectangle((int) tile.X, (int) tile.Y + 8, 8, 2),
                        rect, Color.White);
                }

                if (tile.West != null && tile.West.TileType != tile.TileType &&
                    (tile.West.TileProperties?.Name.Length > tile.TileProperties?.Name.Length ||
                     tile.West.TileProperties == null))
                {
                    var rect = new Rectangle(0, 2, 2, 8);
                    GraphicsUtils.Instance.SpriteBatch.Draw(tileSet,
                        new Rectangle((int) tile.X - 2, (int) tile.Y, 2, 8),
                        rect, Color.White);
                }
            }
        }

        public void DrawWorldObjects(IGameArea gameArea, Camera camera)
        {
            foreach (var entity in gameArea.WorldObjects)
            {
                var worldObjectComponent = entity.GetComponent<WorldObjectManagerComponent>();
                
                if (worldObjectComponent.WorldObjectData.BuildNodes != null &&
                    worldObjectComponent.WorldObjectData.BuildNodes.Length > 0)
                    DrawNodes(entity, worldObjectComponent);
                
                GraphicsUtils.Instance.SpriteBatch.Draw(worldObjectComponent.WorldObjectTexture,
                    entity.GetCurrentBounds(), Color.White);
            }

            var clientPlayer = gameArea.GameWorld.RequestClientPlayer?.Invoke();

            var itemPlacerComponent = clientPlayer?.GetComponent<ItemPlacerComponent>();
            if (itemPlacerComponent?.SelectedItem == null) return;

            var selectedItem = itemPlacerComponent.SelectedItem;
            var texture = ContentChest.ItemTextures[selectedItem.ItemId];
            GraphicsUtils.Instance.SpriteBatch.Draw(texture, clientPlayer.Center, Color.White);
        }

        private static void DrawNodes(IEntity entity, WorldObjectManagerComponent worldObjectComponent)
        {
            foreach (var node in worldObjectComponent.WorldObjectData.BuildNodes)
            {
                var nodeXOffset = node.X * worldObjectComponent.WorldObjectTexture.Width -
                                  ContentChest.NodeTexture.Width / 2.0f;
                var nodeYOffset = node.Y * worldObjectComponent.WorldObjectTexture.Height -
                                  ContentChest.NodeTexture.Height / 2.0f;

                GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.NodeTexture, 
                    entity.Position + new Vector2(nodeXOffset, nodeYOffset), 
                    Color.White);
            }
        }
    }
}