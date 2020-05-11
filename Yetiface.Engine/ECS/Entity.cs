﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Yetiface.Engine.ECS.Components;
using Yetiface.Engine.Screens;

namespace Yetiface.Engine.ECS
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; } = 1.0f;
        public IScreen Screen { get; set; }
        
        public HashSet<IComponent> Components { get; set; }

        public T AddComponent<T>(T component) where T : IComponent
        {
            if(Components == null)
                Components = new HashSet<IComponent>();

            component.Entity = this;
            Components.Add(component);

            return component;
        }

        public void Update()
        {
            if (Components == null) return;
            
            foreach (var component in Components)
                component.Update();
        }

        public void Draw()
        {
            if (Components == null) return;
            
            foreach (var component in Components)
                component.Draw();
        }

        public void DrawDebug()
        {
            
        }
    }
}