using System.Collections.Generic;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Mayday.Game.Graphics
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

    public class Animation : IAnimation
    {
        private readonly Texture2D _image;
        private IList<ISprite> _sprites;

        private float frameDuration = 0.100f;
        private float passedTime;
        private int currentFrameNumber;

        public Animation(Texture2D image)
        {
            _image = image;
        }

        public void Initialize(string filePath)
        {
            _sprites = new List<ISprite>();
            var JsonFrames = JsonConvert.DeserializeObject< JsonFrames>(filePath);
            
            foreach (var frame in JsonFrames.Frames)
            {
                Sprite sprite = new Sprite(_image, new Rectangle(frame.Value.Frame.X, frame.Value.Frame.Y, frame.Value.Frame.W, frame.Value.Frame.H));
                _sprites.Add(sprite);
            }
        }

        public void Update()
        {
            passedTime += Time.DeltaTime;
            if (passedTime >= frameDuration)
            {
                passedTime = 0;
                currentFrameNumber++;

                if (currentFrameNumber == _sprites.Count)
                {
                    currentFrameNumber = 0;
                }
            }
        }

        public void Draw()
        {
            GraphicsUtils.Instance.Draw(_sprites[currentFrameNumber], Window.BottomRight + new Vector2(-50, -50), 0, 3, Color.White);
        }
    }
}