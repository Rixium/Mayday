﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Items;
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
        public static Dictionary<string, Item> ItemData;

        public static Dictionary<string, TileProperties> TileProperties { get; set; } =
            new Dictionary<string, TileProperties>();

        // ReSharper disable once CollectionNeverUpdated.Global
        public static Dictionary<int, SpriteSheet> Heads { get; set; } = new Dictionary<int, SpriteSheet>();

        public static Dictionary<string, Texture2D> TileTextures { get; set; } =
            new Dictionary<string, Texture2D>();

        public static Dictionary<string, Texture2D> ItemTextures { get; set; } =
            new Dictionary<string, Texture2D>();

        public static Dictionary<string, Texture2D> WorldObjectTextures { get; set; } =
            new Dictionary<string, Texture2D>();

        // ReSharper disable once CollectionNeverQueried.Global
        public static Dictionary<string, SoundEffect> SoundEffects { get; set; } =
            new Dictionary<string, SoundEffect>();

        public static Dictionary<string, WorldObjectData> WorldObjectData { get; set; }
            = new Dictionary<string, WorldObjectData>();

        public void Load(ContentManager contentManager)
        {
            LoadDictionary(contentManager, "Images\\Tiles", TileTextures);
            LoadDictionary(contentManager, "Images\\Items", ItemTextures);

            LoadJsonImages(contentManager);
            LoadSoundEffects(contentManager);
            LoadTileProperties(contentManager);
            LoadItemData(contentManager);
            LoadWorldObjectData(contentManager);

            Background = contentManager.Load<Texture2D>("Images\\background");
            NodeTexture = contentManager.Load<Texture2D>("Images\\NodeTexture");
        }

        public static Texture2D Background { get; set; }
        public static Texture2D NodeTexture { get; set; }

        private static void LoadWorldObjectData(ContentManager contentManager)
        {
            var filePath = $"{contentManager.RootDirectory}\\Data\\WorldObjectData.json";
            var worldObjectData = File.ReadAllText(filePath);
            WorldObjectData = JsonConvert.DeserializeObject<Dictionary<string, WorldObjectData>>(worldObjectData);
        }

        private static void LoadDictionary(ContentManager contentManager, string pathRelativeToContent,
            Dictionary<string, Texture2D> dictionary)
        {
            var directory = $"{contentManager.RootDirectory}\\{pathRelativeToContent}";

            var imageFiles = Directory.GetFiles(directory, "*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            foreach (var file in imageFiles)
            {
                dictionary.Add(file,
                    contentManager.Load<Texture2D>($"{pathRelativeToContent}\\{file}"));
            }
        }

        private static void LoadSoundEffects(ContentManager contentManager)
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

        private static void LoadTileProperties(ContentManager contentManager)
        {
            var tilePropertiesFile = $"{contentManager.RootDirectory}\\Data\\TileProperties.json";
            var tilePropertiesData = File.ReadAllText(tilePropertiesFile);

            TileProperties = JsonConvert.DeserializeObject<Dictionary<string, TileProperties>>(tilePropertiesData);
        }

        private static void LoadItemData(ContentManager contentManager)
        {
            var itemDataFile = $"{contentManager.RootDirectory}\\Data\\ItemData.json";
            var itemDataFileText = File.ReadAllText(itemDataFile);

            ItemData = JsonConvert.DeserializeObject<Dictionary<string, Item>>(itemDataFileText);

            foreach (var item in ItemData.Values)
            {
                if (item.WorldObjectType == null) continue;
                var texture = GetWorldObjectTexture(contentManager, item.WorldObjectType);
                if (WorldObjectTextures.ContainsKey(item.WorldObjectType)) continue;
                WorldObjectTextures.Add(item.WorldObjectType, texture);
            }
        }

        private static Texture2D
            GetWorldObjectTexture(ContentManager contentManager, string worldObjectType) =>
            contentManager.Load<Texture2D>($"Images\\WorldObjects\\{worldObjectType}");

        private void LoadJsonImages(ContentManager contentManager)
        {
            var directory = $"{contentManager.RootDirectory}\\Images\\Animations";

            var imageFiles = Directory.GetFiles(directory, "*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            var jsonFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(Path.GetFileName).ToArray();

            foreach (var file in imageFiles.Where(fileName => jsonFiles.Contains(fileName)))
            {
                var texture = contentManager.Load<Texture2D>($"Images\\Animations\\{file}");
                LoadAnimation(directory, file, texture);
            }
        }


        private void LoadAnimation(string folder, string fileName, Texture2D texture)
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