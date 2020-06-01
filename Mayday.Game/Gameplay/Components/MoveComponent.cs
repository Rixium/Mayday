using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class MoveComponent : IComponent
    {
        public IPlayer Player { get; set; }

        private float _yVelocity;
        private float _xVelocity;

        public void Update()
        {
            var gameWorld = Player.GameWorld;

            var oldX = Player.X;

            _yVelocity = MathHelper.Lerp(-11, _yVelocity, 0.99f);

            var tileBelow = gameWorld.GetTileAt(Player.GetBounds().X / gameWorld.TileSize,
                (Player.GetBounds().Bottom - (int) _yVelocity) / gameWorld.TileSize);
            
            if (tileBelow == null || 
                tileBelow.TileType != TileType.NONE)
                _yVelocity = 0;

            tileBelow = gameWorld.GetTileAt(Player.GetBounds().X / gameWorld.TileSize,
                (Player.GetBounds().Bottom) / gameWorld.TileSize + 1);

            if (Player.XDirection != 0 && tileBelow != null && tileBelow.TileType != TileType.NONE)
                _xVelocity += Player.XDirection * 5 * Time.DeltaTime;
            else if(tileBelow == null || tileBelow.TileType != TileType.NONE)
                _xVelocity = MathHelper.Lerp(0, _xVelocity, 0.5f);
            else
                _xVelocity = MathHelper.Lerp(0, _xVelocity, 0.99999f);

            _xVelocity = MathHelper.Clamp(_xVelocity, -2, 2);

            var xMove = (int)_xVelocity;

            var yMove = (int)-_yVelocity;

            gameWorld.Move(Player, xMove, yMove);
        }
        
    }
}