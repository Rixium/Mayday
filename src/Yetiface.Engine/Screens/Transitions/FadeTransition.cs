﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Screens.Transitions
{

    public class FadeTransition : Transition
    {
        private Texture2D _pixel;
        private float _fade = 1.0f;

        public FadeTransition()
        {
            _pixel = YetiGame.ContentManager.Load<Texture2D>("Utils/pixel");
        }
        
        /// <summary>
        /// Transition in will fade in a black cover over the screen.
        /// </summary>
        protected override void TransitionIn()
        {
            _fade = MathHelper.Clamp(_fade - Time.DeltaTime, 0, 1);
            
            if (_fade <= 0.0f)
            {
                OnTransitionInComplete?.Invoke();
            }
        }

        /// <summary>
        /// Transition out will fade out the black cover.
        /// </summary>
        protected override void TransitionOut()
        {
            _fade = MathHelper.Clamp(_fade +  Time.DeltaTime, 0, 1);

            if (_fade >= 1.0f)
            {
                OnTransitionOutComplete?.Invoke();
            }
        }
        
        /// <summary>
        /// Draw the cover.
        /// </summary>
        public override void Draw()
        {
            GraphicsUtils.Instance.Begin();
            GraphicsUtils.Instance.DrawFilledRectangle(0, 0, Window.ViewportWidth, Window.ViewportHeight, Color.Black * _fade);
            GraphicsUtils.Instance.End();
        }

    }
}