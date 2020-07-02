using System;
using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Yetiface.Engine.Inputs;

namespace Mayday.Game.Gameplay.Creators
{
    public class PlayerCreationResult : IPlayerCreationResult
    {
        public IEntity Player { get; set; }
        public IList<Action<MouseButton>> MouseDown { get; set; }
    }
}