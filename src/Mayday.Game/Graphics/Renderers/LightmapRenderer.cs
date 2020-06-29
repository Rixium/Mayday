using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class LightmapRenderer
    {
        private RenderTarget2D _renderTarget;

        public void RenderToRenderTarget(float[,] lightMap, IGameWorld gameWorld, IEntity player)
        {
            _renderTarget = new RenderTarget2D(Window.GraphicsDeviceManager.GraphicsDevice, lightMap.GetLength(0), lightMap.GetLength(1), false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsUtils.Instance.Begin();
            for (var i = 0; i < lightMap.GetLength(0); i++)
            {
                for (var j = 0; j < lightMap.GetLength(1); j++)
                {
                    if (Math.Abs(lightMap[i, j]) < 0.01f) continue;

                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        GraphicsUtils.Instance.PixelTexture,
                        new Rectangle(i, j, 1, 1),
                        Color.Black * lightMap[i, j]);
                }
            }

            GraphicsUtils.Instance.End();
            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);
        }
        public void Draw(IGameWorld gameWorld, IEntity player, Camera camera)
        {
            GraphicsUtils.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, // Only render images when end has been called
                BlendState.AlphaBlend,  // No blending
                null, // Point clamp, so we get sexy pixel perfect resizing
                null, // We don't care about this. Tbh, I don't even understand it.
                null, // I don't even know what this it.
                null, // We can choose to flip textures as an example, but we dont, so null it.
                camera.GetMatrix() * Window.ViewportMatrix); // Window viewport, for nice resizing.);
            GraphicsUtils.Instance.SpriteBatch.Draw(_renderTarget,
                new Rectangle(0, 0, player.GameArea.AreaWidth * gameWorld.TileSize,
                    player.GameArea.AreaHeight * gameWorld.TileSize), Color.White);
            GraphicsUtils.Instance.End();
        }
    }
}