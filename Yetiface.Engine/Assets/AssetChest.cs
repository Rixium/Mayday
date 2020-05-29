using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Yetiface.Engine.Assets
{
    public class AssetChest<T> : IAssetChest
    {
        
        private Dictionary<int, T> _assets = new Dictionary<int,T>();
        
        public T this[int index]
        {
            get
            {
                _assets.TryGetValue(index, out var texture);
                return texture;
            }
        }
        
        public void Load(ContentManager contentManager, string path)
        {
            var allFiles = Directory.GetFiles(contentManager.RootDirectory, "*.xnb", SearchOption.AllDirectories)
                .Select(Path.GetFileName);

            foreach (var file in allFiles.Where(m => m.Contains(path)))
            {
                var split = file.Split('.');
                var fileName = split[0];
                var lastIndexOf = fileName.LastIndexOf('_');
                var fileNumber = int.Parse(fileName.Substring(lastIndexOf + 1));
                var subDirectory = "";
                if (typeof(T) == typeof(Texture2D))
                {
                    subDirectory = "Images/";
                }

                var loaded = contentManager.Load<T>($"{subDirectory}{fileName}");
                _assets.Add(fileNumber, loaded);
            }
        }
    }
}