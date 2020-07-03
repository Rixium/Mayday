using Mayday.Game.Gameplay.Collections;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Gameplay.Items;
using Mayday.Game.Gameplay.World;
using Mayday.Game.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Optimization;
using Yetiface.Engine.Utils;

namespace Mayday.Game.Graphics.Renderers
{
    public class GameRenderer : IGameRenderer
    {
        private readonly IPlayerRenderer _playerRenderer;
        private readonly IWorldRenderer _worldRenderer;
        private readonly ILightMapRenderer _lightMapRenderer;
        private readonly IUpdateResolver<IEntity> _updateResolver;

        private RenderTarget2D _renderTarget = new RenderTarget2D(
            Window.GraphicsDeviceManager.GraphicsDevice,
            Window.WindowWidth, Window.WindowHeight);

        public GameRenderer(IPlayerRenderer playerRenderer,
            IWorldRenderer worldRenderer,
            ILightMapRenderer lightMapRenderer,
            IUpdateResolver<IEntity> updateResolver)
        {
            _playerRenderer = playerRenderer;
            _worldRenderer = worldRenderer;
            _lightMapRenderer = lightMapRenderer;
            _updateResolver = updateResolver;
        }

        public void Draw(
            Camera camera,
            IGameWorld gameWorld,
            IEntitySet entitySet,
            IEntity myPlayer,
            LightMap lightMap)
        {

            if(_renderTarget.Width != Window.WindowWidth)
                _renderTarget = new RenderTarget2D(
                Window.GraphicsDeviceManager.GraphicsDevice,
                Window.WindowWidth, Window.WindowHeight);

            if (lightMap.ChangedSinceLastGet)
                _lightMapRenderer.RenderToRenderTarget(
                    camera,
                    myPlayer.GameArea,
                    lightMap);

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsUtils.Instance.Begin();
            GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.Background, new Rectangle(0, 0, Window.WindowWidth, Window.WindowHeight), Color.White);
            GraphicsUtils.Instance.End();

            GraphicsUtils.Instance.Begin(camera.GetMatrix());
            _worldRenderer.DrawWorldObjects(gameWorld.GameAreas[0], camera);
            _playerRenderer.DrawPlayers(entitySet.GetAll());

            foreach (var entity in gameWorld.GameAreas[0].GetItems())
            {
                if (!_updateResolver.ShouldUpdate(entity)) continue;
                if (!(entity is ItemDrop item)) continue;
                GraphicsUtils.Instance.SpriteBatch.Draw(ContentChest.ItemTextures[item.Item.ItemId],
                    new Vector2(item.X, item.Y), Color.White);
            }

            _worldRenderer.Draw(gameWorld.GameAreas[0], camera);
            GraphicsUtils.Instance.End();

            GraphicsUtils.Instance.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,  // No blending
                null, // Point clamp, so we get sexy pixel perfect resizing
                      null, // We don't care about this. Tbh, I don't even understand it.
                null, // I don't even know what this it.
                null, // We can choose to flip textures as an example, but we dont, so null it.
                camera.GetMatrix()); // Window viewport, for nice resizing.
            _lightMapRenderer?.Draw(camera, myPlayer, gameWorld);
            GraphicsUtils.Instance.End();

            Window.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            GraphicsUtils.Instance.SpriteBatch.Begin();
            GraphicsUtils.Instance.SpriteBatch.Draw(_renderTarget,
                new Rectangle(0, 0, Window.WindowWidth, Window.WindowHeight), Color.White);
            GraphicsUtils.Instance.End();
        }

    }
}