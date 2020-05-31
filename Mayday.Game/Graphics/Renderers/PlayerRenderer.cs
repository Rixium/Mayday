using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
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

            if (legSprite != null)
                DrawSprite(legSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(bodySprite != null)
                DrawSprite(bodySprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(headSprite != null)
                DrawSprite(headSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            var name = SteamFriends.GetFriendPersona(player.SteamId);
            var nameSize = GraphicsUtils.Instance.DebugFont.MeasureString(name);
            var drawPos = new Vector2( player.GetBounds().X + player.GetBounds().Width / 2.0f - nameSize.X / 2.0f / 2.0f, player.GetBounds().Y - 20 - nameSize.Y / 2.0f);
            
            GraphicsUtils.Instance.DrawFilledRectangle((int) drawPos.X - 5, (int) drawPos.Y - 5, (int)( nameSize.X * 0.5f) + 10, (int) (nameSize.Y * 0.5f) + 10, Color.Black * 0.5f);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont,
                name, drawPos, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0F);
        }

        private void DrawSprite(ISprite sprite, Vector2 playerPosition, SpriteEffects flip)
        {
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture, playerPosition, sprite.SourceRectangle, Color.White,
                0, Vector2.Zero, 1f, flip, 0F);
        }
    }
}