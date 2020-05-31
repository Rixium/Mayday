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
            var armSprite = player.ArmsAnimator?.Current;
            var legSprite = player.LegsAnimator?.Current;
            var playerPosition = new Vector2(player.X, player.Y);
            var flip = player.XDirection < 0;
            
            if(armSprite != null)
                GraphicsUtils.Instance.SpriteBatch.Draw(
                    armSprite.Texture, playerPosition, armSprite.SourceRectangle, Color.White,
                    0, Vector2.Zero, 1f, flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0F);
            if (legSprite != null)
                DrawSprite(legSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(bodySprite != null)
                DrawSprite(bodySprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(headSprite != null)
                DrawSprite(headSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if(armSprite != null)
                DrawSprite(armSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            var name = SteamFriends.GetFriendPersona(player.SteamId);
            var nameSize = GraphicsUtils.Instance.DebugFont.MeasureString(name);
            var drawX = new Vector2(player.X + headSprite.Texture.Width / 2.0f - nameSize.X / 2.0f,
                player.Y - 5 - nameSize.Y * 0.5f);
            
            GraphicsUtils.Instance.DrawFilledRectangle((int) drawX.X - 5, (int) drawX.Y - 5, (int)( nameSize.X * 0.5f) + 10, (int) (nameSize.Y * 0.5f) + 10, Color.Black * 0.5f);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont,
                name, new Vector2(player.X + headSprite.Texture.Width / 2.0f - nameSize.X / 2.0f, player.Y - 5 - nameSize.Y * 0.5f), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0F);
        }

        private void DrawSprite(ISprite sprite, Vector2 playerPosition, SpriteEffects flip)
        {
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture, playerPosition, sprite.SourceRectangle, Color.White,
                0, Vector2.Zero, 1f, flip, 0F);
        }
    }
}