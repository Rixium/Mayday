using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Screens;
using Yetiface.Engine.Inputs;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Gameplay.Components
{
    public class ItemPlacerComponent : IComponent
    {
        private GameScreen _gameScreen;
        private Camera _camera => _gameScreen.Camera;
        public IGameWorld GameWorld => _gameScreen.GameWorld;
        public IEntity Entity { get; set; }
        public int MaxDistanceToPlace => 7 * GameWorld.TileSize;
        
        public ItemPlacerComponent(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        public void OnAddedToEntity()
        {
            
        }

        public void MouseDown(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                var selectedItem = _gameScreen.SelectedItem;
                if (selectedItem == null) return;
                if (selectedItem.TileId == -1) return;
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());

                var mouseTileX = mousePosition.X / GameWorld.TileSize;
                var mouseTileY = mousePosition.Y / GameWorld.TileSize;

                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > GameWorld.Width - 1 ||
                    mouseTileY > GameWorld.Height - 1) return;
                if (!CanPlaceAt(mouseTileX, mouseTileY)) return;
                var tile = GameWorld.Tiles[mouseTileX, mouseTileY];

                if (!CloseEnoughToTile(tile)) return;
                GameWorld.PlaceTile(tile, selectedItem.TileId);
            }
        }

        private bool CanPlaceAt(int tileX, int tileY)
        {
            return GameWorld.Tiles[tileX, tileY].TileType == 0;
        }

        private bool CloseEnoughToTile(Tile tile)
        {
            var playerBounds = Entity.GetBounds();
            var playerLeft = playerBounds.Left;
            var playerRight = playerBounds.Right;
            var playerTop = playerBounds.Top;
            var playerBottom = playerBounds.Bottom;
            
            if (Math.Abs(tile.RenderCenter.X - playerLeft) > MaxDistanceToPlace) return false;
            if (Math.Abs(tile.RenderCenter.X - playerRight) > MaxDistanceToPlace) return false;
            if (Math.Abs(tile.RenderCenter.Y - playerTop) > MaxDistanceToPlace) return false;
            return (Math.Abs(tile.RenderCenter.Y - playerBottom) <= MaxDistanceToPlace);
        }
    }
}