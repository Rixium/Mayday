﻿using System;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework.Audio;
using Yetiface.Engine;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Utils;
using MouseState = Yetiface.Engine.Utils.MouseState;

namespace Mayday.Game.Gameplay.Components
{
    public class BlockBreakerComponent : IComponent
    {
        private readonly Camera _camera;
        private double _lastBreak;

        public IEntity Entity { get; set; }
        public IGameWorld GameWorld { get; set; }

        public int MaxDistanceToBreak => 20 * GameWorld.TileSize;
        
        public BlockBreakerComponent(IGameWorld gameWorld, Camera camera)
        {
            GameWorld = gameWorld;
            _camera = camera;
        }
        
        private bool CloseEnoughToTile(Tile tile)
        {
            var playerBounds = Entity.GetCurrentBounds();
            var playerLeft = playerBounds.Left;
            var playerRight = playerBounds.Right;
            var playerTop = playerBounds.Top;
            var playerBottom = playerBounds.Bottom;
            
            if (Math.Abs(tile.RenderCenter.X - playerLeft) > MaxDistanceToBreak) return false;
            if (Math.Abs(tile.RenderCenter.X - playerRight) > MaxDistanceToBreak) return false;
            if (Math.Abs(tile.RenderCenter.Y - playerTop) > MaxDistanceToBreak) return false;
            return (Math.Abs(tile.RenderCenter.Y - playerBottom) <= MaxDistanceToBreak);
        }

        public void OnAddedToEntity()
        {
            
        }

        public void MouseDown(MouseButton button)
        {
            if (button == MouseButton.Right && Time.GameTime.TotalGameTime.TotalSeconds > _lastBreak + 0.5f)
            {
                var mousePosition = MouseState.Bounds(_camera.GetMatrix());

                var mouseTileX = mousePosition.X / GameWorld.TileSize;
                var mouseTileY = mousePosition.Y / GameWorld.TileSize;
                if (mouseTileX < 0 || mouseTileY < 0 || mouseTileX > Entity.GameArea.AreaWidth - 1 ||
                    mouseTileY > Entity.GameArea.AreaHeight - 1) return;
                
                var tile = Entity.GameArea.Tiles[mouseTileX, mouseTileY];

                if (!CloseEnoughToTile(tile)) return;
                if (!TileCanBeBroken(tile)) return;

                YetiGame.ContentManager.Load<SoundEffect>("dig").Play();
                
                tile.Break();
                _lastBreak = Time.GameTime.TotalGameTime.TotalSeconds;
            }
        }

        private bool TileCanBeBroken(Tile tile)
        {
            if (tile.TileType == TileTypes.None) return false;
            return Entity.GameArea.GetWorldObjectAbove(tile) == null;
        }
    }
}