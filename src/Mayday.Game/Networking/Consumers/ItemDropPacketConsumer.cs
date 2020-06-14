using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Networking.Consumers
{
    public class ItemDropPacketConsumer : PacketConsumer<ItemDropPacket>
    {
        private readonly GameScreen _gameScreen;

        public ItemDropPacketConsumer(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }
        
        protected override void ConsumePacket(Connection connection, ItemDropPacket packet)
        {
            var itemDrop = new ItemDrop
            {
                Item = ContentChest.ItemData[(ItemType) packet.ItemId],
                X = packet.X,
                Y = packet.Y
            };

            var moveComponent = itemDrop.AddComponent(new MoveComponent());
            itemDrop.AddComponent(new GravityComponent());
            moveComponent.XVelocity = Randomizer.Next(-10, 10);
            moveComponent.YVelocity = Randomizer.Next(0, 5);

            _gameScreen.DropItem(itemDrop);
        }
    }
}