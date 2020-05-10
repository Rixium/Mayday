using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Graphics
{
    public class JsonFrames
    {
        public Dictionary<string, JsonAnimData> Frames { get; set; }
    }

    public class JsonAnimData
    {
        public JsonFrameData Frame { get; set; }
        public int Duration { get; set; }
    }

    public class JsonFrameData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }

    public class Animation : SpriteRenderComponent
    {
        private readonly IList<ISprite> _sprites;

        private const float FrameDuration = 0.100f;
        private float _passedTime;
        private int _currentFrameNumber;

        public Animation(Texture2D image, string path) : base(null)
        {
            _sprites = new List<ISprite>();
            
            var data = File.ReadAllText(path);
            var jsonFrames = JsonConvert.DeserializeObject<JsonFrames>(data);
            
            foreach (var sprite in jsonFrames.Frames.Select(frame => new Sprite(image, new Rectangle(frame.Value.Frame.X, frame.Value.Frame.Y, frame.Value.Frame.W, frame.Value.Frame.H))))
            {
                _sprites.Add(sprite);
            }

            Sprite = _sprites[0];
        }

        public override void Update()
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