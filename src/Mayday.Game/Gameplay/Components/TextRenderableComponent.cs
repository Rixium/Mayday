using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Gameplay.Components
{
    public class TextRenderableComponent : IRenderable
    {
        private string _activeText;
        private Vector2 _position;
        public IEntity Entity { get; set; }

        public void Show(Vector2 position, string text)
        {
            _position = position;
            _activeText = text;
        }

        public void Hide() => _activeText = null;

        public void OnAddedToEntity()
        {

        }

        public void Draw()
        {
            if (_activeText == null) return;

            GraphicsUtils.Instance.SpriteBatch.DrawString(
                    GraphicsUtils.Instance.DebugFont,
                    _activeText,
                    _position,
                    Color.White
                );
        }

    }
}