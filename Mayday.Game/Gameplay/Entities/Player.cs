using System.Collections.Generic;
using Mayday.Game.Gameplay.Components;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Gameplay.Entities
{
    public class Player : IPlayer
    {
        public ulong SteamId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int HeadId { get; set; } = 1;
        public int BodyId { get; set; } = 1;
        public int LegsId { get; set; } = 1;
        public int ArmsId { get; set; } = 1;

        public IAnimator HeadAnimator { get; set; }
        public IAnimator BodyAnimator { get; set; }
        public IAnimator LegsAnimator { get; set; }

        public int XDirection { get; set; }

        public int FacingDirection { get; set; } = 1;
        public IGameWorld GameWorld { get; set; }

        private IList<IComponent> _components = new List<IComponent>();

        public void Update()
        {
            HeadAnimator?.Update();
            BodyAnimator?.Update();
            LegsAnimator?.Update();
            //
            // if (oldX != X)
            // {
            //     HeadAnimator?.SetAnimation("Walk");
            //     BodyAnimator?.SetAnimation("Walk");
            //     LegsAnimator?.SetAnimation("Walk");
            // }
            // else
            // {
            //     HeadAnimator?.StopAnimation();
            //     BodyAnimator?.StopAnimation();
            //     LegsAnimator?.StopAnimation();
            // }

            foreach (var component in _components)
                component.Update();
        }

        public Rectangle GetBounds() =>
            new Rectangle(X + 18, Y + 18,
                LegsAnimator.Current.SourceRectangle.Value.Width - 17 - 18,
                LegsAnimator.Current.SourceRectangle.Value.Height - 19);

        public IComponent AddComponent(IComponent component)
        {
            component.Player = this;
            _components.Add(component);
            return component;
        }
        
    }
}