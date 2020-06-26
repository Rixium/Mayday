using System;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework.Audio;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPlacerComponent : IComponent
    {
        private GameScreen _gameScreen;

        private double _lastPlaced;
        private Camera _camera => _gameScreen.Camera;
        public IGameWorld GameWorld => _gameScreen.GameWorld;
        public IEntity Entity { get; set; }
        public int MaxDistanceToPlace => 7 * GameWorld.TileSize;
        public Action<IItem> ItemUsed { get; set; }
        public IItem SelectedItem { get; set; }

        public ItemPlacerComponent(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        public void OnAddedToEntity()
        {
        }

        public void MouseDown(MouseButton button)
        {
            if (button == MouseButton.Left && Time.GameTime.TotalGameTime.TotalSeconds > _lastPlaced + 0.5f)
            {
                if (SelectedItem == null) return;

                if (SelectedItem.TileType == null || SelectedItem.TileType == TileTypes.None)
                {
                    PlaceAsWorldObject();
                    return;
                }

                PlaceAsTile();
            }
        }

        private void PlaceAsTile()
        {
            var mousePosition = MouseState.Bounds(_camera.GetMatrix());

            var mouseTileX = mousePosition.X / GameWorld.TileSize;
            var mouseTileY = mousePosition.Y / GameWorld.TileSize;

            if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > Entity.GameArea.AreaWidth - 1 ||
                mouseTileY > Entity.GameArea.AreaHeight - 1) return;
            if (!CanPlaceAt(mouseTileX, mouseTileY, true)) return;
            var tile = Entity.GameArea.Tiles[mouseTileX, mouseTileY];
            if (!CloseEnoughToTile(tile)) return;
            Entity.GameArea.PlaceTile(tile, SelectedItem.TileType);
            YetiGame.ContentManager.Load<SoundEffect>("place").Play();
            ItemUsed?.Invoke(SelectedItem);
            _lastPlaced = Time.GameTime.TotalGameTime.TotalSeconds;
        }

        private void PlaceAsWorldObject()
        {
            var mousePosition = MouseState.Bounds(_camera.GetMatrix());

            var mouseTileX = mousePosition.X / GameWorld.TileSize;
            var mouseTileY = mousePosition.Y / GameWorld.TileSize;

            if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > Entity.GameArea.AreaWidth - 1 ||
                mouseTileY > Entity.GameArea.AreaHeight - 1) return;

            var tile = Entity.GameArea.Tiles[mouseTileX, mouseTileY];
            if (!CloseEnoughToTile(tile)) return;
            if (WorldObjectIntersectsSomethingAt(SelectedItem, tile)) return;

            Entity.GameArea.PlaceWorldEntity(tile, SelectedItem.WorldObjectType);
            YetiGame.ContentManager.Load<SoundEffect>("place").Play();
            ItemUsed?.Invoke(SelectedItem);
            _lastPlaced = Time.GameTime.TotalGameTime.TotalSeconds;
        }

        private bool WorldObjectIntersectsSomethingAt(IItem selectedItem, Tile tile)
        {
            var worldObjectTexture = ContentChest.ItemTextures[selectedItem.ItemId];

            for (var i = tile.TileX; i <= tile.TileX + worldObjectTexture.Width * Game1.GlobalGameScale / GameWorld.TileSize; i++)
            {
                for (var j = tile.TileY; j <= tile.TileY + worldObjectTexture.Height * Game1.GlobalGameScale / GameWorld.TileSize; j++)
                {
                    if (!CanPlaceAt(i, j)) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether an item can be placed at a particular tileX and tileY position.
        /// </summary>
        /// <param name="tileX">The TileX position (i.e tile index).</param>
        /// <param name="tileY">The TileY position (i.e tile index).</param>
        /// <param name="requiresImmediateNeighbour">Whether or not the item requires a neighbour to be placed.</param>
        /// <returns>Returns whether or not the item can be placed at the given position.</returns>
        private bool CanPlaceAt(int tileX, int tileY, bool requiresImmediateNeighbour = false)
        {
            var tile = Entity.GameArea.TryGetTile(tileX, tileY);

            if (tile == null) return false;
            
            if (requiresImmediateNeighbour && !tile.HasImmediateNeighbour()) 
                return false;

            return tile.TileType == TileTypes.None &&
                !GameWorld.AnythingCollidesWith(tile);
        }

        private bool CloseEnoughToTile(Tile tile)
        {
            var playerBounds = Entity.GetCurrentBounds();
            var playerLeft = playerBounds.Left;
            var playerRight = playerBounds.Right;
            var playerTop = playerBounds.Top;
            var playerBottom = playerBounds.Bottom;
            
            if (Math.Abs(tile.RenderCenter.X - playerLeft) > MaxDistanceToPlace) return false;
            if (Math.Abs(tile.RenderCenter.X - playerRight) > MaxDistanceToPlace) return false;
            if (Math.Abs(tile.RenderCenter.Y - playerTop) > MaxDistanceToPlace) return false;
            return (Math.Abs(tile.RenderCenter.Y - playerBottom) <= MaxDistanceToPlace);
        }

        public void SetSelectedItem(IItem item) => SelectedItem = item;
    }
}