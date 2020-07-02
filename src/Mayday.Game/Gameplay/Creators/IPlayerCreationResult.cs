using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Yetiface.Engine.Inputs;

namespace Mayday.Game.Gameplay.Creators
{
    public interface IPlayerCreationResult
    {
        IEntity Player { get; set; }
        IList<Action<MouseButton>> MouseDown { get; set; }
    }
}