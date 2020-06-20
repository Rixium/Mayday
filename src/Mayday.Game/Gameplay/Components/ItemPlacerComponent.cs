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
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPlacerComponent : IComponent
    {
        private GameScreen _gameScreen;
        private IItem _selectedItem;
        private double _lastPlaced;
        private Camera _camera => _gameScreen.Camera;
        public IGameWorld GameWorld => _gameScreen.GameWorld;
        public IEntity Entity { get; set; }
        public int MaxDistanceToPlace => 7 * GameWorld.TileSize;
        public Action<IItem> ItemUsed { get; set; }

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
                if (_selectedItem == null) return;

                if (_selectedItem.TileType == TileType.None)
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

            if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > GameWorld.Width - 1 ||
                mouseTileY > GameWorld.Height - 1) return;
            if (!CanPlaceAt(mouseTileX, mouseTileY)) return;
            var tile = GameWorld.Tiles[mouseTileX, mouseTileY];
            if (!CloseEnoughToTile(tile)) return;
            GameWorld.PlaceTile(tile, _selectedItem.TileType);
            YetiGame.ContentManager.Load<SoundEffect>("place").Play();
            ItemUsed?.Invoke(_selectedItem);
            _lastPlaced = Time.GameTime.TotalGameTime.TotalSeconds;
        }

        private void PlaceAsWorldObject()
        {
            var mousePosition = MouseState.Bounds(_camera.GetMatrix());

            var mouseTileX = mousePosition.X / GameWorld.TileSize;
            var mouseTileY = mousePosition.Y / GameWorld.TileSize;

            if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > GameWorld.Width - 1 ||
                mouseTileY > GameWorld.Height - 1) return;

            var tile = GameWorld.Tiles[mouseTileX, mouseTileY];
            if (!CloseEnoughToTile(tile)) return;
            if (WorldObjectIntersectsSomethingAt(_selectedItem, tile)) return;

            GameWorld.PlaceWorldEntity(tile, _selectedItem.WorldObjectType);
            YetiGame.ContentManager.Load<SoundEffect>("place").Play();
            ItemUsed?.Invoke(_selectedItem);
            _lastPlaced = Time.GameTime.TotalGameTime.TotalSeconds;
        }

        private bool WorldObjectIntersectsSomethingAt(IItem selectedItem, Tile tile)
        {
            var worldObjectTexture = ContentChest.ItemTextures[selectedItem.ItemId];

            for (var i = tile.TileX; i < tile.TileX + worldObjectTexture.Width / GameWorld.TileSize; i++)
            {
                for (var j = tile.TileY; j < tile.TileY + worldObjectTexture.Height / GameWorld.TileSize; j++)
                {
                    if (!CanPlaceAt(i, j)) return true;
                }
            }

            return false;
        }

        private bool CanPlaceAt(int tileX, int tileY)
        {
            return GameWorld.Tiles[tileX, tileY].TileType == TileType.None &&
                !GameWorld.AnythingCollidesWith(GameWorld.Tiles[tileX, tileY]);
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

        public void SetSelectedItem(IItem item) => _selectedItem = item;
    }
}