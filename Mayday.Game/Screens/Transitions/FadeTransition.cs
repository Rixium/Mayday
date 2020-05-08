using System;
using Mayday.Game.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mayday.Game.Screens.Transitions
{

    public class FadeTransition : Transition
    {
        private Texture2D _pixel;

        private float _fade = 1.0f;
        public float Speed { get; set; } = 1.0f;

        public FadeTransition()
        {
            _pixel = Game1._pixel;
        }
        
        /// <summary>
        /// Transition in will fade in a black cover over the screen.
        /// </summary>
        protected override void TransitionIn()
        {
            _fade -= Time.DeltaTime * Speed;

            _fade = MathHelper.Clamp(_fade, 0, 1);
            
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
            _fade += Time.DeltaTime * Speed;

            _fade = MathHelper.Clamp(_fade, 0, 1);
            
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
            GraphicsUtils.SpriteBatch.Draw(_pixel, new Rectangle(0, 0, Window.WindowWidth, Window.ViewportHeight),
                Color.Black * _fade);
        }
        
    }
}