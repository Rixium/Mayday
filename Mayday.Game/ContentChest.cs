using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        
        public static Dictionary<int, TileProperties> TileProperties { get; set; } = new Dictionary<int, TileProperties>();
        public static Dictionary<int, SpriteSheet> Heads { get; set; } = new Dictionary<int, SpriteSheet>();
        public static Dictionary<int, SpriteSheet> Bodies { get; set; } = new Dictionary<int, SpriteSheet>();
        public static Dictionary<int, SpriteSheet> Legs { get; set; } = new Dictionary<int, SpriteSheet>();
        public static Dictionary<int, Texture2D> Tiles { get; set; } = new Dictionary<int, Texture2D>();
        
        public static Dictionary<string, Texture2D> TileToTiles { get; set; } = new Dictionary<string, Texture2D>();

        public static Dictionary<string, SoundEffect> SoundEffects { get; set; } =
            new Dictionary<string, SoundEffect>();

        public void Load(ContentManager contentManager)
        {
            LoadTiles(contentManager);
            LoadJsonImages(contentManager);
            LoadSoundEffects(contentManager);
            LoadTileProperties(contentManager);
        }

        private void LoadTiles(ContentManager contentManager)
        {
            var directory = $"{contentManager.RootDirectory}\\Images";

            var imageFiles = Directory.GetFiles(directory, "Tiles*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            foreach (var file in imageFiles)
            {
                var nameOf = file.Split('_');
                Tiles.Add(int.Parse(nameOf[1]), contentManager.Load<Texture2D>($"Images\\{file}"));
            }
        }
        
        private void LoadSoundEffects(ContentManager contentManager)
        {
            var directory = $"{contentManager.RootDirectory}\\Game\\Sounds";
            var soundFiles = Directory.GetFiles(directory, "*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            foreach (var file in soundFiles)
            {
                SoundEffects.Add(file, contentManager.Load<SoundEffect>("Game\\Sounds\\" + file));
            }
        }

        private void LoadTileProperties(ContentManager contentManager)
        {
            var tilePropertiesFile = $"{contentManager.RootDirectory}\\Data\\TileProperties.json";
            var tilePropertiesData = File.ReadAllText(tilePropertiesFile);
            
            TileProperties = JsonConvert.DeserializeObject<Dictionary<int, TileProperties>>(tilePropertiesData);
        }

        private void LoadJsonImages(ContentManager contentManager)
        {
            var directory = $"{contentManager.RootDirectory}\\Images";

            var imageFiles = Directory.GetFiles(directory, "*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            var jsonFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            foreach (var file in imageFiles.Where(fileName => jsonFiles.Contains(fileName)))
            {
                var texture = contentManager.Load<Texture2D>($"Images\\{file}");
                LoadAnimation(contentManager, directory, file, texture);
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
                
                spriteSheet.Animations.Add(frameTag.Name, new Animation(new List<ISprite>(sprites)));
                
                sprites.Clear();
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