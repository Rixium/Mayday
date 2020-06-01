using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Graphics;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class PlayerRenderer : IPlayerRenderer
    {
        
        public Color OutlineColor { get; set;  }= new Color(24, 24, 24);
        
        public void DrawPlayers(Dictionary<ulong, IPlayer> players)
        {
            foreach(var player in players)
                DrawPlayer(player.Value);
        }

        public void DrawPlayer(IPlayer player)
        {
            var headSprite = player.HeadAnimator?.Current;
            var bodySprite = player.BodyAnimator?.Current;
            var legSprite = player.LegsAnimator?.Current;
            var playerPosition = player.Position;
            var flip = player.FacingDirection < 0;
            
            if(bodySprite != null)
                DrawSprite(bodySprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, true);
            if(headSprite != null)
                DrawSprite(headSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, true);

            if (legSprite != null)
                DrawSprite(legSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(bodySprite != null)
                DrawSprite(bodySprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(headSprite != null)
                DrawSprite(headSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            
        }
        
        private void DrawSprite(ISprite sprite, Vector2 playerPosition, SpriteEffects flip, bool isBorder = false)
        {
            if (isBorder)
                DrawAsBorder(sprite, playerPosition, flip);
            else
                GraphicsUtils.Instance.SpriteBatch.Draw(
                    sprite.Texture, playerPosition, sprite.SourceRectangle, Color.White,
                    0, Vector2.Zero, 1f, flip, 0F);
        }

        private void DrawAsBorder(ISprite sprite, Vector2 playerPosition, SpriteEffects flip)
        {

            var current = playerPosition + Vector2.UnitX;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, OutlineColor,
                0f, Vector2.Zero, 1f, flip, 0F);
            
            current = playerPosition - Vector2.UnitX;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, OutlineColor,
                0f, Vector2.Zero, 1f, flip, 0F);
            
            current = playerPosition + Vector2.UnitY;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, OutlineColor,
                0f, Vector2.Zero, 1f, flip, 0F);
            
            current = playerPosition - Vector2.UnitY;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, OutlineColor,
                0f, Vector2.Zero, 1f, flip, 0F);
        }
    }
}