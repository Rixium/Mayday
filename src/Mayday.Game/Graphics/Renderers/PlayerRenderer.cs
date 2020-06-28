using System.Collections.Generic;
using Mayday.Game.Gameplay.Components;
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
        private IDictionary<ulong, PlayerAnimationComponent> _playerAnimationComponents = 
            new Dictionary<ulong, PlayerAnimationComponent>();
        
        public void DrawPlayers(IEnumerable<IEntity> players)
        {
            foreach(var player in players)
                DrawPlayer(player);
        }

        public void DrawPlayer(IEntity player)
        {
            var playerAnimationComponent = GetPlayerAnimationComponent(player);
            
            var headSprite = playerAnimationComponent.HeadAnimator?.Current;
            
            var playerPosition = player.Position;
            var flip = player.FacingDirection < 0;

            if(headSprite != null)
                DrawSprite(headSprite, playerPosition, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

        }

        private PlayerAnimationComponent GetPlayerAnimationComponent(IEntity player)
        {
            _playerAnimationComponents.TryGetValue(player.EntityId, out var component);

            if (component != null) return component;

            component = player.GetComponent<PlayerAnimationComponent>();
            _playerAnimationComponents.Add(player.EntityId, component);
            return component;
        }

        private static void DrawSprite(ISprite sprite, Vector2 playerPosition, SpriteEffects flip)
        {
            GraphicsUtils.Instance.SpriteBatch.Draw(
                sprite.Texture, playerPosition, sprite.SourceRectangle, Color.White,
                0, Vector2.Zero,  1, flip, 0F);
        }

    }
}