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

            _renderTarget ??= new RenderTarget2D(Window.GraphicsDeviceManager.GraphicsDevice, lightMapData.GetLength(0),
                lightMapData.GetLength(1),
                false,
                Window.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _renderTarget.GraphicsDevice.Clear(Color.Transparent);

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_renderTarget);

            var startX = (int) camera.Bounds.Left / gameArea.GameWorld.TileSize;
            var startY =(int) camera.Bounds.Top / gameArea.GameWorld.TileSize;
            var endX = (int)camera.Bounds.Right / gameArea.GameWorld.TileSize;
            var endY = (int)camera.Bounds.Bottom / gameArea.GameWorld.TileSize;

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
                    if (i >= lightMapData.GetLength(0)) continue;
                    if (j >= lightMapData.GetLength(1)) continue;

                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        GraphicsUtils.Instance.PixelTexture,
                        new Rectangle(i, j, 1, 1),
                        Color.Black * lightMapData[i, j]);
                }
            }

            GraphicsUtils.Instance.End();

            lightMap.ChangedSinceLastGet = false;
        }
        
        public void Draw(IEntity player, IGameWorld gameWorld)
        {
            if (_renderTarget == null) return;
            GraphicsUtils.Instance.SpriteBatch.Draw(_renderTarget,
                new Rectangle(0, 0, player.GameArea.AreaWidth * gameWorld.TileSize, player.GameArea.AreaHeight * gameWorld.TileSize),
                Color.White);
        }

    }
}