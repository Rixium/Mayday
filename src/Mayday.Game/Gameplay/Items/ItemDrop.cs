using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Items
{
    public class ItemDrop : Entity
    {

        public Item Item { get; set; }

        public override RectangleF GetBounds()
        {
            var texture = ContentChest.ItemTextures[(ItemType) Item.Id];
            return new RectangleF(X, Y, texture.Width, texture.Height);
        }
        
    }
}