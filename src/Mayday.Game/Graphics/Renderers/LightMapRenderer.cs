using System;
using System.Diagnostics;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class LightMapRenderer
    {
        private RenderTarget2D _renderTarget;

        public void RenderToRenderTarget(float[,] lightMap)
        {
            if (lightMap == null) return;

            _renderTarget ??= new RenderTarget2D(Window.GraphicsDeviceManager.GraphicsDevice, lightMap.GetLength(0),
                lightMap.GetLength(1),
                false,
                Window.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _renderTarget.GraphicsDevice.Clear(Color.Transparent);

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsUtils.Instance.SpriteBatch.Begin(
                SpriteSortMode.Deferred, // Only render images when end has been called
                BlendState.Opaque,  // No blending
                null, // Point clamp, so we get sexy pixel perfect resizing
                DepthStencilState.Default, // We don't care about this. Tbh, I don't even understand it.
                null, // I don't even know what this it.
                null, // We can choose to flip textures as an example, but we dont, so null it.
                null); // Window viewport, for nice resizing.
            for (var i = 0; i < lightMap.GetLength(0); i++)
            {
                for (var j = 0; j < lightMap.GetLength(1); j++)
                {
                    GraphicsUtils.Instance.SpriteBatch.Draw(
                        GraphicsUtils.Instance.PixelTexture,
                        new Rectangle(i, j, 1, 1),
                        Color.Black * lightMap[i, j]);
                }
            }

            GraphicsUtils.Instance.End();
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