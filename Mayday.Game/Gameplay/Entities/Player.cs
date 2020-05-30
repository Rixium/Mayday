using Mayday.Game.Graphics;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player : IPlayer
    {
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

        public void Update()
        {
            HeadAnimator?.Update();
            BodyAnimator?.Update();
            LegsAnimator?.Update();
            ArmsAnimator?.Update();

            var oldX = X;
            X += XDirection;

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
        
    }
}