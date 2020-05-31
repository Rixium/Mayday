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
            var playerPosition = new Vector2(player.X, player.Y);
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


            foreach (var message in player.GetChat())
            {
                ShowMessage(message);
            }
        }

        private void ShowMessage(ChatMessage message)
        {
            var text = message.Text;
            var textSize = GraphicsUtils.Instance.DebugFont.MeasureString(text);
            var drawPos = new Vector2( message.X- (int)(textSize.X * 0.25f / 2), message.Y);
            
            GraphicsUtils.Instance.DrawFilledRectangle((int) drawPos.X - 5, (int) drawPos.Y - 5, (int)( textSize.X * 0.25f) + 10, (int) (textSize.Y * 0.25f) + 10, Color.Black * 0.5f * message.Fade);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont,
                text, drawPos, Color.White * message.Fade, 0, Vector2.Zero, 0.25f, SpriteEffects.None, 0F);
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
                sprite.SourceRectangle, Color.Black,
                0f, Vector2.Zero, 1f, flip, 0F);
            
            current = playerPosition - Vector2.UnitX;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, Color.Black,
                0f, Vector2.Zero, 1f, flip, 0F);
            
            current = playerPosition + Vector2.UnitY;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, Color.Black,
                0f, Vector2.Zero, 1f, flip, 0F);
            
            current = playerPosition - Vector2.UnitY;
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture,
                current,
                sprite.SourceRectangle, Color.Black,
                0f, Vector2.Zero, 1f, flip, 0F);
        }
    }
}