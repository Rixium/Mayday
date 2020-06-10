
using System;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{
    public interface IEntity
    {
        Action<IEntity> Destroy { get; set; }
        IGameWorld GameWorld { get; set; }
        int XDirection { get; set; }
        float X { get; set; }
        float Y { get; set; }
        Vector2 Position { get; }
        Vector2 Center { get; }
        int FacingDirection { get; set; }
        RectangleF GetBounds();
        void Update();
        T AddComponent<T>(T component) where T : IComponent;
        T GetComponent<T>() where T : IComponent;
    }
}