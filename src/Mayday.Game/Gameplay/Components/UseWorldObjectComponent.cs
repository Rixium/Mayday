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
        public Action<IEntity> UseWorldObject;

        private readonly IEntity _player;
        private WorldObjectData _worldObjectData;

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
            if (PlayerIsCloseEnough())
            {
                InRangeOfWorldObject?.Invoke(Entity);
            }
        }

        private bool PlayerIsCloseEnough() =>
            Vector2.Distance(_player.Center, Entity.Center) <=
            RangeToUseInTiles * Game1.GlobalGameScale * Entity.GameWorld.TileSize;

        public void Use() =>
            UseWorldObject?.Invoke(Entity);

    }
}