using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Graphics.Renderers
{
    public class BasicRenderer : IRenderer
    {
        private readonly IScreen _screen;

        public BasicRenderer(IScreen screen)
        {
            _screen = screen;
        }

        public void Draw()
        {
            if (_screen.Renderables == null) return;

            foreach (var renderable in _screen.Renderables)
                GraphicsUtils.Instance.Draw(renderable.Sprite, renderable.Entity.Position, renderable.Entity.Rotation,
                    renderable.Entity.Scale, renderable.Color);
        }
    }
}