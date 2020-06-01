using System;
using System.Collections.Generic;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics
{
    public class Animator : IAnimator
    {
        
        // A reference to all the animations that the entity this animator is
        // connected to will require.
        private readonly Dictionary<string, Animation> _animations;

        private string _currentAnimation = "Walk";
        private int _frameNumber;
        private float _currentFrameTime;

        public Animation ActiveAnimation => 
            _animations.ContainsKey(_currentAnimation) ? _animations[_currentAnimation] : null;

        public ISprite Current => ActiveAnimation?
            .Sprites[_frameNumber];
            
        public bool Animating { get; set; }

        public Animator(Dictionary<string, Animation> animations)
        {
            _animations = new Dictionary<string, Animation>();
            foreach(var pair in animations)
                _animations.Add(pair.Key, pair.Value);
        }

        public void SetAnimation(string animationName)
        {
            Animating = true;

            if (animationName.Equals(_currentAnimation)) return;
            
            _currentAnimation = animationName;
            _frameNumber = 0;
            _currentFrameTime = 0;
        }

        public void Update(float frameSpeed)
        {
            if (!Animating) return;
            
            _currentFrameTime += Time.DeltaTime;

            if (_currentFrameTime < frameSpeed) return;

            NextFrame();
        }

        public void StopAnimation()
        {
            Animating = false;
            _frameNumber = 0;
            _currentFrameTime = 0;
        }

        private void NextFrame()
        {
            _frameNumber++;
            _currentFrameTime = 0;

            if (_frameNumber >= ActiveAnimation.FrameCount)
                _frameNumber = 0;
        }
        
    }
}