
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Yetiface.Engine.Data;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;
using IUpdateable = Yetiface.Engine.ECS.Components.Updateables.IUpdateable;

namespace Yetiface.Engine.ECS.Components.Renderables
{
    public class Animation : SpriteRenderComponent, IUpdateable
    {
        private readonly IList<ISprite> _sprites;

        private const float FrameDuration = 0.100f;
        private float _passedTime;
        private int _currentFrameNumber;

        public Animation(Texture2D image, string path) : base(null)
        {
            _sprites = new List<ISprite>();
            
            var data = File.ReadAllText(path);
            var jsonFrames = JsonConvert.DeserializeObject<AsepriteSheet>(data);

            foreach (var frame in jsonFrames.Frames)
            {
                var sprite = new Sprite(image, new Rectangle(frame.Value.Frame.X, frame.Value.Frame.Y, frame.Value.Frame.W, frame.Value.Frame.H));
                _sprites.Add(sprite);
            }

            Sprite = _sprites[0];
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