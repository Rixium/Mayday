﻿namespace Yetiface.Engine.ECS.Components
{
    public interface IComponent
    {
        IEntity Entity { get; set; }
    }
}