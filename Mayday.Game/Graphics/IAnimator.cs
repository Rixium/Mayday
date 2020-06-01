using Yetiface.Engine.Graphics;

namespace Mayday.Game.Graphics
{
    public interface IAnimator
    {
        void SetAnimation(string animationName);
        void Update(float frameSpeed);
        void StopAnimation();
        ISprite Current { get; }
    }
}