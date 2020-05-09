using System.Collections.Generic;
using Mayday.Game.ECS.Components;
using Mayday.Game.Screens;
using Microsoft.Xna.Framework;

namespace Mayday.Game.ECS
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