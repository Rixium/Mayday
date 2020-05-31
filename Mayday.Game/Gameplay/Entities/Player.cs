using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player : IPlayer
    {
        private float _yVelocity;
        private float _xVelocity;
        
        public ulong SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int HeadId { get; set; } = 1;
        public int BodyId { get; set; } = 1;
        public int LegsId { get; set; } = 1;
        public int ArmsId { get; set; } = 1;
        
        public IAnimator HeadAnimator { get; set; }
        public IAnimator BodyAnimator { get; set; }
        public IAnimator LegsAnimator { get; set; }
        public IAnimator ArmsAnimator { get; set; }
        public int XDirection { get; set; }
        public IGameWorld GameWorld { get; set; }

        public void Update()
        {
            HeadAnimator?.Update();
            BodyAnimator?.Update();
            LegsAnimator?.Update();
            ArmsAnimator?.Update();

            var oldX = X;
            
            if (_yVelocity > -11)
                _yVelocity -= 10 * Time.DeltaTime;

            if (XDirection > 0)
                _xVelocity += 10 * Time.DeltaTime;
            else if (XDirection < 0)
                _xVelocity -= 10 * Time.DeltaTime;
            else _xVelocity = Vector2.Lerp(new Vector2(0, 0), new Vector2(_xVelocity, 0), 0.95f).X;

            _xVelocity = MathHelper.Clamp(_xVelocity, -2, 2);
            
            var tileBelow = GameWorld.GetTileAt(GetBounds().X / GameWorld.TileSize,
                (GetBounds().Bottom - (int) _yVelocity) / GameWorld.TileSize);
            
            if (tileBelow == null || 
                tileBelow.TileType != TileType.NONE)
                _yVelocity = 0;

            var xMove = (int)_xVelocity;

            var yMove = (int)-_yVelocity;

            GameWorld?.Move(this, xMove, yMove);

            if (oldX != X)
            {
                HeadAnimator?.SetAnimation("Walk");
                BodyAnimator?.SetAnimation("Walk");
                LegsAnimator?.SetAnimation("Walk");
                ArmsAnimator?.SetAnimation("Walk");
            }
            else
            {
                HeadAnimator?.StopAnimation();
                BodyAnimator?.StopAnimation();
                LegsAnimator?.StopAnimation();
                ArmsAnimator?.StopAnimation();
            }
        }

        public void Jump()
        {
            _yVelocity = 6;
        }

        public Rectangle GetBounds() =>
            new Rectangle(X + 16, Y + 3,
                HeadAnimator.Current.SourceRectangle.Value.Width - 31,
                HeadAnimator.Current.SourceRectangle.Value.Height - 4);
    }
}