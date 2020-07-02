using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Mayday.Game.Networking;
using Mayday.Game.UI.Controllers;
using Yetiface.Engine.Inputs;
using Yetiface.Engine.Optimization;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Creators
{
    public class PlayerCreator : IPlayerCreator
    {
        private readonly IGameWorld _gameWorld;
        private readonly GameScreenUserInterfaceController _interfaceController;
        private readonly Camera _camera;
        private readonly IUpdateResolver<IEntity> _updateResolver;

        public PlayerCreator(
            IGameWorld gameWorld,
            GameScreenUserInterfaceController interfaceController,
            Camera camera,
            IUpdateResolver<IEntity> updateResolver)
        {
            _gameWorld = gameWorld;
            _interfaceController = interfaceController;
            _camera = camera;
            _updateResolver = updateResolver;
        }

        public IPlayerCreationResult CreateHostPlayer(IEntity player)
        {
            player.GameWorld = _gameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[1].Animations)
            };

            var moveComponent = player.AddComponent(new MoveComponent());
            var gravityComponent = player.AddComponent(new GravityComponent());
            var jumpComponent = player.AddComponent(new JumpComponent());
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));
            var itemPickerComponent =
                player.AddComponent(new ItemPickerComponent(_gameWorld.GameAreas[0].WorldItems, _updateResolver));
            playerAnimationComponent = player.AddComponent(playerAnimationComponent);

            moveComponent.PositionChanged += PacketManager.SendPositionPacket;
            moveComponent.MoveDirectionChanged += PacketManager.SendMoveDirectionPacket;

            player.Bounds = new RectangleF(9, 10, 13, 30);

            var blockBreakerComponent = player.AddComponent(new BlockBreakerComponent(_gameWorld, _camera));
            player.AddComponent(new CharacterControllerComponent());
            var itemPlacerComponent = player.AddComponent(new ItemPlacerComponent(_camera));
            _interfaceController.SelectedItemSlotChanged += (i) =>
            {
                var item = inventoryBar.GetItemAt(i);
                itemPlacerComponent.SetSelectedItem(item);
            };
            itemPlacerComponent.ItemUsed += (item) => inventoryBar.RemoveItem(item);
            inventoryBar.InventoryChanged += () => _interfaceController.InventoryBarChanged(inventoryBar);
            mainInventory.InventoryChanged += () => _interfaceController.MainInventoryChanged(mainInventory);
            jumpComponent.Jump += PacketManager.SendJumpPacket;
            inventoryBar.AddItemToInventory(ContentChest.ItemData["Shuttle"]);

            player.GameArea = _gameWorld.GameAreas[0];

            var spawnTile = _gameWorld.GameAreas[0].GetRandomSpawnLocation();
            player.X = spawnTile.TileX * _gameWorld.TileSize;
            player.Y = spawnTile.TileY * _gameWorld.TileSize - player.Bounds.Height ;

            _gameWorld.AddTrackedEntity(player);

            return new PlayerCreationResult
            {
                Player = player,
                MouseDown = new List<Action<MouseButton>>
                {
                    blockBreakerComponent.MouseDown,
                    itemPlacerComponent.MouseDown
                }
            };
        }

        public IPlayerCreationResult CreateClientPlayer(IEntity player)
        {
            player.GameWorld = _gameWorld;

            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[1].Animations)
            };

            var moveComponent = player.AddComponent(new MoveComponent());
            player.AddComponent(new GravityComponent());
            player.AddComponent(new JumpComponent());
            var inventoryComponent = player.AddComponent(new InventoryComponent());
            inventoryComponent.AddInventory(new Inventory(8));
            inventoryComponent.AddInventory(new Inventory(24));
            player.AddComponent(new ItemPickerComponent(_gameWorld.GameAreas[0].WorldItems, _updateResolver));
            player.AddComponent(playerAnimationComponent);

            moveComponent.PositionChanged += PacketManager.SendPositionPacket;
            moveComponent.MoveDirectionChanged += PacketManager.SendMoveDirectionPacket;

            player.Bounds = new RectangleF(9, 10, 13, 30);
            player.GameArea = _gameWorld.GameAreas[0];

            _gameWorld.AddTrackedEntity(player);

            return new PlayerCreationResult
            {
                Player = player
            };
        }
    }
}