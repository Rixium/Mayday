using System.Collections.Generic;
using System.Linq;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Graphics;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Entities
{

    public class ChatMessage
    {

        public float Y;
        public float Fade = 1;
        public string Text;
        public float X;
        
        public void Update()
        {
            Fade -= MathHelper.Lerp(0, Fade, Time.DeltaTime);
            Y -= Time.DeltaTime * 10;
        }
        
    }
    public class Player : IPlayer
    {
        private float _yVelocity;
        private float _xVelocity;
        private IList<ChatMessage> _chatMessages = new List<ChatMessage>();

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

        public void Update()
        {
            HeadAnimator?.Update();
            BodyAnimator?.Update();
            LegsAnimator?.Update();
            
            var oldX = X;

            _yVelocity = MathHelper.Lerp(-11, _yVelocity, 0.99f);

            var tileBelow = GameWorld.GetTileAt(GetBounds().X / GameWorld.TileSize,
                (GetBounds().Bottom - (int) _yVelocity) / GameWorld.TileSize);
            
            if (tileBelow == null || 
                tileBelow.TileType != TileType.NONE)
                _yVelocity = 0;

            tileBelow = GameWorld.GetTileAt(GetBounds().X / GameWorld.TileSize,
                (GetBounds().Bottom) / GameWorld.TileSize + 1);

            if (XDirection != 0 && tileBelow != null && tileBelow.TileType != TileType.NONE)
                _xVelocity += XDirection * 5 * Time.DeltaTime;
            else if(tileBelow == null || tileBelow.TileType != TileType.NONE)
                _xVelocity = MathHelper.Lerp(0, _xVelocity, 0.5f);
            else
                _xVelocity = MathHelper.Lerp(0, _xVelocity, 0.99999f);

            _xVelocity = MathHelper.Clamp(_xVelocity, -2, 2);

            var xMove = (int)_xVelocity;

            var yMove = (int)-_yVelocity;

            GameWorld?.Move(this, xMove, yMove);

            if (oldX != X)
            {
                HeadAnimator?.SetAnimation("Walk");
                BodyAnimator?.SetAnimation("Walk");
                LegsAnimator?.SetAnimation("Walk");
            }
            else
            {
                HeadAnimator?.StopAnimation();
                BodyAnimator?.StopAnimation();
                LegsAnimator?.StopAnimation();
            }

            var array = _chatMessages.ToArray();
            
            foreach (var message in array)
            {
                message.Update();
                
                if (message.Fade <= 0)
                {
                    _chatMessages.Remove(message);
                }
            }
            
        }

        public void Jump()
        {
            _yVelocity = 5;
        }

        public Rectangle GetBounds() =>
            new Rectangle(X + 18, Y + 18,
                LegsAnimator.Current.SourceRectangle.Value.Width - 17 - 18,
                LegsAnimator.Current.SourceRectangle.Value.Height - 19);

        public void AddChat(string receivedMessage)
        {
            _chatMessages.Add(new ChatMessage
            {
                Text = receivedMessage,
                X = GetBounds().X,
                Y = GetBounds().Top - 10
            });
        }

        public IList<ChatMessage> GetChat()
        {
            return _chatMessages;
        }
    }
}