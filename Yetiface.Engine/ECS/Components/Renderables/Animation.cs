using System.Collections.Generic;
using Yetiface.Engine.ECS.Components.Updateables;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.ECS.Components.Renderables
{
    public class Animation : SpriteRenderComponent, IUpdateable
    {
        private readonly IList<ISprite> _sprites;

        private const float FrameDuration = 0.100f;
        private float _passedTime;
        private int _currentFrameNumber;

        public Animation(IList<ISprite> sprites) : base(null)
        {
            _sprites = sprites;
        }

        public void Update()
        {
            _passedTime += Time.DeltaTime;
            if (_passedTime < FrameDuration) return;

            _passedTime = 0;
            _currentFrameNumber++;

            if (_currentFrameNumber == _sprites.Count)
            {
                _currentFrameNumber = 0;
            }

            Sprite = _sprites[_currentFrameNumber];
        }
    }
}