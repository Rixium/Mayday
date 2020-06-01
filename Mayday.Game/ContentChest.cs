using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Yetiface.Engine.Data;
using Yetiface.Engine.ECS.Components.Renderables;
using Yetiface.Engine.Graphics;

namespace Mayday.Game
{
    
    public class ContentChest
    {
        
        public static Dictionary<int, SpriteSheet> Heads { get; set; } = new Dictionary<int, SpriteSheet>();
        public static Dictionary<int, SpriteSheet> Bodies { get; set; } = new Dictionary<int, SpriteSheet>();
        public static Dictionary<int, SpriteSheet> Legs { get; set; } = new Dictionary<int, SpriteSheet>();
        public static Dictionary<int, Texture2D> Tiles { get; set; } = new Dictionary<int, Texture2D>();

        public void Load(ContentManager contentManager)
        {
            LoadImages(contentManager);
        }

        private void LoadImages(ContentManager contentManager)
        {
            var directory = $"{contentManager.RootDirectory}\\Images";

            var imageFiles = Directory.GetFiles(directory, "*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            var jsonFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            foreach (var file in imageFiles)
            {
                var texture = contentManager.Load<Texture2D>($"Images\\{file}");
                
                // It's an animation if we have a json file associated with it. (Same name, different extension).
                if (jsonFiles.Contains(file))
                {
                    LoadAnimation(contentManager, directory, file, texture);
                }
                else
                {
                    var properties = typeof(ContentChest).GetProperties();
                    var nameOf = file.Split('_');

                    foreach (var property in properties)
                    {
                        if (!property.Name.Equals(nameOf[0], StringComparison.OrdinalIgnoreCase)) continue;
                        var actualProperty = (Dictionary<int, Texture2D>) property.GetValue(this, null);
                        actualProperty.Add(int.Parse(nameOf[1]), texture);
                        break;
                    }
                }
            }
        }

        private void LoadAnimation(ContentManager contentManager, string folder, string fileName, Texture2D texture)
        {
            var sheetText = File.ReadAllText($"{folder}\\{fileName}.json");
            var sheetData = JsonConvert.DeserializeObject<AsepriteSheet>(sheetText);
            
            // The sprite sheet to hold everything about our animations and stuff.
            var spriteSheet = new SpriteSheet
            {
                Texture = texture
            };

            var sprites = new List<ISprite>();
            
            foreach (var frameTag in sheetData.Meta.FrameTags)
            {
                // Create a new list of sprites to store information about each frame.

                for (var i = frameTag.From; i <= frameTag.To; i++)
                {
                    var frameData = sheetData.Frames[i];
                    var sprite = new Sprite(texture,
                        new Rectangle(frameData.Frame.X, frameData.Frame.Y, frameData.Frame.W, frameData.Frame.H));
                    sprites.Add(sprite);
                }
                
                spriteSheet.Animations.Add(frameTag.Name, new Animation(sprites));
            }

            var properties = typeof(ContentChest).GetProperties();
            var nameOf = fileName.Split('_');

            foreach (var property in properties)
            {
                if (!property.Name.Equals(nameOf[0], StringComparison.OrdinalIgnoreCase)) continue;
                var actualProperty = (Dictionary<int, SpriteSheet>) property.GetValue(this, null);
                actualProperty.Add(int.Parse(nameOf[1]), spriteSheet);
                break;
            }
        }
        
    }
    
}