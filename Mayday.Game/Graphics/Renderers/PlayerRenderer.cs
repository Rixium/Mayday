using System.Collections.Generic;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
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

            if(armSprite != null)
                GraphicsUtils.Instance.SpriteBatch.Draw(
                    armSprite.Texture, playerPosition, armSprite.SourceRectangle, Color.White,
                    0, armSprite.Origin, 1f, SpriteEffects.FlipHorizontally, 0F);
            if (legSprite != null)
                GraphicsUtils.Instance.Draw(legSprite, playerPosition, Color.White);
            if(bodySprite != null)
                GraphicsUtils.Instance.Draw(bodySprite, playerPosition, Color.White);
            if(headSprite != null)
                GraphicsUtils.Instance.Draw(headSprite, playerPosition, Color.White);
            if(armSprite != null)
                GraphicsUtils.Instance.Draw(armSprite, playerPosition, Color.White);

            var name = SteamFriends.GetFriendPersona(player.SteamId);
            var nameSize = GraphicsUtils.Instance.DebugFont.MeasureString(name);
            GraphicsUtils.Instance.SpriteBatch.DrawString(GraphicsUtils.Instance.DebugFont,
                name, new Vector2(player.X + headSprite.Texture.Width / 2.0f - nameSize.X / 2.0f, player.Y - 20 - nameSize.Y), Color.Black, 0, new Vector2(nameSize.X / 2.0f, nameSize.
                    Y / 2.0f), 1, SpriteEffects.None, 0F);
        }
    }
}