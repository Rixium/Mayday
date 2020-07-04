using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Collections;
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

            var playerComponentSet = AddCommonComponents(player);
            AddHostComponents(player, playerComponentSet);
            player.Bounds = new RectangleF(9, 10, 13, 30);

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
                    playerComponentSet.BlockBreakerComponent.MouseDown,
                    playerComponentSet.ItemPlacerComponent.MouseDown
                }
            };
        }

        private void AddHostComponents(IEntity player, PlayerComponentSet playerComponentSet)
        {
            playerComponentSet.BlockBreakerComponent = player.AddComponent(new BlockBreakerComponent(_gameWorld, _camera));
            playerComponentSet.CharacterControllerComponent = player.AddComponent(new CharacterControllerComponent());
            playerComponentSet.ItemPlacerComponent = player.AddComponent(new ItemPlacerComponent(_camera)
            {
                ItemUsed = (item) => playerComponentSet.BarInventory.RemoveItem(item)
            });


            _interfaceController.SelectedItemSlotChanged += (i) =>
            {
                var item = playerComponentSet.BarInventory.GetItemAt(i);
                playerComponentSet.ItemPlacerComponent.SetSelectedItem(item);
            };

            playerComponentSet.BarInventory.InventoryChanged +=
                () => _interfaceController.InventoryBarChanged(playerComponentSet.BarInventory);
            playerComponentSet.MainInventory.InventoryChanged +=
                () => _interfaceController.MainInventoryChanged(playerComponentSet.MainInventory);
            playerComponentSet.JumpComponent.Jump += PacketManager.SendJumpPacket;
            playerComponentSet.BarInventory.AddItemToInventory(ContentChest.ItemData["Shuttle"]);
        }

        private PlayerComponentSet AddCommonComponents(IEntity player)
        {
            var playerAnimationComponent = new PlayerAnimationComponent
            {
                HeadAnimator = new Animator(ContentChest.Heads[1].Animations)
            };

            var inventoryComponent = player.AddComponent(new InventoryComponent());
            var inventoryBar = inventoryComponent.AddInventory(new Inventory(8));
            var mainInventory = inventoryComponent.AddInventory(new Inventory(24));

            return new PlayerComponentSet
            {
                MoveComponent = player.AddComponent(new MoveComponent
                {
                    PositionChanged = PacketManager.SendPositionPacket,
                    MoveDirectionChanged = PacketManager.SendMoveDirectionPacket
                }),
                GravityComponent = player.AddComponent(new GravityComponent()),
                JumpComponent = player.AddComponent(new JumpComponent()),
                MainInventory = mainInventory,
                BarInventory = inventoryBar,
                ItemPickerComponent = player.AddComponent(new ItemPickerComponent(_gameWorld.GameAreas[0].WorldItems, _updateResolver)),
                PlayerAnimationComponent = player.AddComponent(playerAnimationComponent)
            };
        }

        public IPlayerCreationResult CreateClientPlayer(IEntity player)
        {
            player.GameWorld = _gameWorld;

            AddCommonComponents(player);

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