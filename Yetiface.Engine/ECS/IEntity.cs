using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.ECS.Components;
using Yetiface.Engine.Screens;

namespace Yetiface.Engine.ECS
{
    public interface IEntity
    {
        string Name { get; set; }
        Vector2 Position { get; set; }
        float Rotation { get; set; }
        float Scale { get; set; }
        
        IScreen Screen { get; set; }
        HashSet<IComponent> Components { get; set; }
        T AddComponent<T>(T component) where T : IComponent;
        void Update();
        void Draw();
        void DrawDebug();
    }
}