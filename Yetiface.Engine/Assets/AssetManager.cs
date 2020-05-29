using System;
using Microsoft.Xna.Framework.Content;

namespace Yetiface.Engine.Assets
{
    public abstract class AssetManager : IAssetManager
    {
        public void Load(ContentManager contentManager)
        {
            var chests = GetType().GetProperties();
            foreach (var assetChest in chests)
            {
                var typeOf = assetChest.PropertyType;
                if (!typeof(IAssetChest).IsAssignableFrom(typeOf)) continue;
                var subtype = assetChest.PropertyType.GetGenericArguments()[0];
                var genericType = typeof(AssetChest<>).MakeGenericType(subtype);
                var instance = Activator.CreateInstance(genericType);
                assetChest.SetValue(this, instance);
                ((IAssetChest) instance).Load(contentManager, assetChest.Name);
            }
        }
        
    }
}