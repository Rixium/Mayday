﻿
using Mayday.Game.Graphics;

namespace Mayday.Game.Gameplay.Entities
{
    public interface IPlayer
    {
        ulong SteamId { get; set; }
        int X { get; set; }
        int Y { get; set; }

        int HeadId { get; set; }
        int BodyId { get; set; }
        int LegsId { get; set; }
        int ArmsId { get; set; }
        
        IAnimator HeadAnimator { get; set; }
        IAnimator BodyAnimator { get; set; }
        IAnimator LegsAnimator { get; set; }
        IAnimator ArmsAnimator { get; set; }
        int XDirection { get; set; }

        void Update();

    }
}