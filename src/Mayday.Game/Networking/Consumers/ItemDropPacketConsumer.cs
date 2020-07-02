using System;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Networking.Packets;
using Mayday.Game.Screens;
using Steamworks.Data;
using Yetiface.Engine.Networking.Consumers;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Networking.Consumers
{
    public class ItemDropPacketConsumer : PacketConsumer<ItemDropPacket>
    {
        private readonly IGameWorld _gameWorld;

        public ItemDropPacketConsumer(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }
        
        protected override void ConsumePacket(Connection connection, ItemDropPacket packet)
        {
            var itemDrop = new ItemDrop
            {
                Item = ContentChest.ItemData[packet.ItemId],
                X = packet.X,
                Y = packet.Y
            };

            var moveComponent = itemDrop.AddComponent(new MoveComponent());
            itemDrop.AddComponent(new GravityComponent());
            moveComponent.XVelocity = Randomizer.Next(-10, 10);
            moveComponent.YVelocity = Randomizer.Next(0, 5);

            // TODO NETWORKED GAME AREAS
            _gameWorld.GameAreas[0].DropItem(itemDrop);
        }
    }
}