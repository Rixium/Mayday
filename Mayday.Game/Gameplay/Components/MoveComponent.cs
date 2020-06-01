using System;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class MoveComponent : IComponent
    {
        public IPlayer Player { get; set; }

        public float YVelocity { get; set; }
        public float XVelocity { get; private set; }

        public void Update()
        {
            var gameWorld = Player.GameWorld;

            if (Player.XDirection != 0)
                XVelocity += Player.XDirection * 5 * Time.DeltaTime;
            else
                XVelocity += -Math.Sign(XVelocity) * 5 * Time.DeltaTime;
            
            XVelocity = MathHelper.Clamp(XVelocity, -1.5f, 1.5f);

            if (Math.Abs(XVelocity) < 0.1f)
                XVelocity = 0;

            var xMove =  XVelocity;

            var yMove = -YVelocity;

            gameWorld.Move(Player, xMove, yMove);
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}