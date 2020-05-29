using Yetiface.Engine.Graphics;

namespace Mayday.Game.Graphics
{
    public interface IAnimator
    {
        void SetAnimation(string animationName);
        void Update();
        void StopAnimation();
        ISprite Current { get; }
    }
}