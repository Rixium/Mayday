using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Items
{
    public class ItemDrop : Entity
    {

        private static ulong _itemDropEntityIdCounter = 1;
        private static ulong CurrentItemDropEntityId => _itemDropEntityIdCounter++;

        public Item Item { get; set; }

        public override RectangleF GetCurrentBounds()
        {
            var texture = ContentChest.ItemTextures[Item.ItemId];
            return new RectangleF(X, Y, texture.Width, texture.Height);
        }

        public ItemDrop() : base(CurrentItemDropEntityId)
        {

        }

    }
}