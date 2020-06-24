using System;

namespace Mayday.Game.Gameplay.Tutorials
{
    public interface ITutorial
    {
        Action Triggered { get; set; }
    }
}