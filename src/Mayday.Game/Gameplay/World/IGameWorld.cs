using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World.Areas;

namespace Mayday.Game.Gameplay.World
{
    public interface IGameWorld
    {
        Func<IEntity> RequestClientPlayer { get; set; }
        Action<IEntity> PlayerInRangeOfWorldObject { get; set; }
        Action<IEntity> PlayerLeftRangeOfWorldObject { get; set; }
        Action<IRenderable> RenderableComponentAdded { get; set; }
        Action<Tile> TilePlaced { get; set; }
        int TileSize { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        HashSet<IEntity> TrackedEntities { get; }
        IList<IGameArea> GameAreas { get; set; }
        Action<Tile> TileDestroyed { get; set; }
        bool AnythingCollidesWith(Tile gameWorldTile);
        void AddTrackedEntity(IEntity entity);
        void Update();
    }
}