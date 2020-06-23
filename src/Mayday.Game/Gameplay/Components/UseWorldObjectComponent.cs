using System;
using System.Diagnostics;
using Mayday.Game.Gameplay.Data;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Components
{
    public class UseWorldObjectComponent : IUpdateable
    {
        public IEntity Entity { get; set; }

        public int RangeToUseInTiles = 2;
        public Action<IEntity> InRangeOfWorldObject;
        public Action<IEntity> LeftRangeOfWorldObject;
        public Action<IEntity> UseWorldObject;

        private readonly IEntity _player;
        private WorldObjectData _worldObjectData;
        private bool _playerInRange;

        public UseWorldObjectComponent(IEntity player, WorldObjectData worldObjectData)
        {
            _player = player;
            _worldObjectData = worldObjectData;
        }

        public void OnAddedToEntity()
        {

        }

        public void Update()
        {
            if (!PlayerIsCloseEnough())
            {
                PlayerHasLeftRange();
                return;
            }

            if (_playerInRange)
                return;

            _playerInRange = true;
            InRangeOfWorldObject?.Invoke(Entity);
        }

        private void PlayerHasLeftRange()
        {
            if (!_playerInRange || PlayerIsCloseEnough())
                return;

            _playerInRange = false;
            LeftRangeOfWorldObject?.Invoke(Entity);
        }

        private bool PlayerIsCloseEnough() =>
            Vector2.Distance(_player.Center, Entity.Center) <=
            RangeToUseInTiles * Entity.GameWorld.TileSize;

        public void Use()
        {
            if (PlayerIsCloseEnough()) return;

            UseWorldObject?.Invoke(Entity);
        }
    }
}