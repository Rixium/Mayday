using System;
using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World.Areas;

namespace Mayday.Game.Gameplay.World
{
    public class GameWorld : IGameWorld
    {
        public Func<IEntity> RequestClientPlayer { get; set; }
        public Action<IEntity> PlayerInRangeOfWorldObject { get; set; }
        public Action<IEntity> PlayerLeftRangeOfWorldObject { get; set; }
        public Action<IRenderable> RenderableComponentAdded { get; set; }

        public int TileSize { get; set; }

        /// <summary>
        /// The width and height are both in tiles, not pixels.
        /// </summary>
        public int Width { get; set; }
        public int Height { get; set; }
        public Action<Tile> TilePlaced { get; set; }

        /// <summary>
        /// These are the entities that should do an update on every frame, even if they exist in a different area.
        /// </summary>
        public HashSet<IEntity> TrackedEntities { get; } = new HashSet<IEntity>();
        public IList<IGameArea> GameAreas { get; set; } = new List<IGameArea>();

        private HashSet<IEntity> _trackedEntitiesToRemove = new HashSet<IEntity>();

        public GameWorld(IGameArea startingArea)
        {
            AddGameArea(startingArea);
        }

        private void AddGameArea(IGameArea area)
        {
            area.GameWorld = this;
            GameAreas.Add(area);
        }

        public bool AnythingCollidesWith(Tile tile) =>
            TrackedEntities.Any(entity => entity != tile &&
                                        tile.GetCurrentBounds().Intersects(entity.GetCurrentBounds()));

        public void AddTrackedEntity(IEntity entity)
        {
            TrackedEntities.Add(entity);
            entity.Destroy += OnTrackedEntityDestroyed;
        }

        private void OnTrackedEntityDestroyed(IEntity obj) =>
            _trackedEntitiesToRemove.Add(obj);

        public void Update()
        {
            CleanUpTrackedEntities();

            foreach(var entity in TrackedEntities)
                entity.Update();
        }

        private void CleanUpTrackedEntities()
        {
            if (_trackedEntitiesToRemove.Count <= 0) return;

            foreach (var entity in _trackedEntitiesToRemove)
                TrackedEntities.Remove(entity);

            _trackedEntitiesToRemove.Clear();
        }

    }
}