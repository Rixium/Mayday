using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Gameplay.World.Areas;
using Mayday.Game.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class LightMapRenderer : ILightMapRenderer
    {
        private RenderTarget2D _renderTarget;

        public void RenderToRenderTarget(Camera camera, IGameArea gameArea, LightMap lightMap)
        {
            if (lightMap == null) return;

            var lightMapData = lightMap.GetLights();

            var startX = (int) camera.Bounds.Left / gameArea.GameWorld.TileSize;
            var startY =(int) camera.Bounds.Top / gameArea.GameWorld.TileSize;
            var endX = (int)camera.Bounds.Right / gameArea.GameWorld.TileSize;
            var endY = (int)camera.Bounds.Bottom / gameArea.GameWorld.TileSize;

            _renderTarget ??= new RenderTarget2D(Window.GraphicsDeviceManager.GraphicsDevice, lightMapData.GetLength(0),
                lightMapData.GetLength(1),
                false,
                Window.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _renderTarget.GraphicsDevice.Clear(Color.Transparent);

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsUtils.Instance.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                null,
                DepthStencilState.Default,
                null,
                null,
                null);

            for (var i = startX; i <= endX; i++)
            {
                for (var j = startY; j <= endY; j++)
                {
                    if (i >= lightMapData.GetLength(0) || i < 0) continue;
                    if (j >= lightMapData.GetLength(1) || j < 0) continue;

                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        GraphicsUtils.Instance.PixelTexture,
                        new Rectangle(i, j, 1, 1),
                        Color.Black * lightMapData[i, j]);
                }
            }

            GraphicsUtils.Instance.End();

            lightMap.ChangedSinceLastGet = false;
        }
        
        public void Draw(Camera camera, IEntity player, IGameWorld gameWorld)
        {
            if (_renderTarget == null) return;
            GraphicsUtils.Instance.SpriteBatch.Draw(_renderTarget,
                new RectangleF(
                    (int)(camera.Bounds.Left / gameWorld.TileSize) * gameWorld.TileSize,
                    (int)(camera.Bounds.Top / gameWorld.TileSize) * gameWorld.TileSize,
                    (int)(camera.Bounds.Width / gameWorld.TileSize) * gameWorld.TileSize,
                        (int)(camera.Bounds.Height/ gameWorld.TileSize) * gameWorld.TileSize),
                new Rectangle(
                    (int) (camera.Bounds.Left / gameWorld.TileSize),
                    (int)(camera.Bounds.Top / gameWorld.TileSize),
                    (int)(camera.Bounds.Width / gameWorld.TileSize),
                    (int)(camera.Bounds.Height / gameWorld.TileSize)),
                Color.White);
        }

    }
}