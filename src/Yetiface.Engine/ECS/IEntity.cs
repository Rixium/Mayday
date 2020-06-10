using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.ECS.Components;
using Yetiface.Engine.Screens;

namespace Yetiface.Engine.ECS
{
    public interface IEntity
    {
        Vector2 Position { get; set; }
        float Scale { get; set; }
        float Rotation { get; set; }
        IList<IComponent> Components { get; set; }
        IScreen Screen { get; set; }
        void AddComponent(IComponent component);
    }
}