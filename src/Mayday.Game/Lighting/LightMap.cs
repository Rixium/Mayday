using System;
using System.Threading.Tasks;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.World.Areas;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Lighting
{
    public class LightMap
    {

        private float[,] _lightValues;

        public bool ChangedSinceLastGet { get; set; }

        private async void CheckLights(Camera camera, IGameArea gameArea)
        {
            _lightValues ??= new float[gameArea.AreaWidth, gameArea.AreaHeight];

            PreCalculateLightValues(camera, gameArea);
            CalculateLighting(camera, gameArea);

            ChangedSinceLastGet = true;
        }

        private void PreCalculateLightValues(Camera camera, IGameArea gameArea)
        {
            var startX = (int) camera.Bounds.Left / gameArea.GameWorld.TileSize;
            var startY =(int) camera.Bounds.Top / gameArea.GameWorld.TileSize;
            var endX = (int)camera.Bounds.Right / gameArea.GameWorld.TileSize;
            var endY = (int)camera.Bounds.Bottom / gameArea.GameWorld.TileSize;
            for (var tileX = startX; tileX <= endX; tileX++)
            {
                for (var tileY = startY; tileY <= endY; tileY++)
                {
                    if (tileX < 0 || tileX >= _lightValues.GetLength(0)) continue;
                    if (tileY < 0 || tileY >= _lightValues.GetLength(1)) continue;

                    var tile = gameArea.TryGetTile(tileX, tileY);

                    if (tile.TileType == TileTypes.None)
                    {
                        _lightValues[tileX, tileY] = 0;
                    }
                    else
                    {
                        _lightValues[tileX, tileY] = 1f;
                    }
                }
            }
        }

        private void CalculateLighting(Camera camera, IGameArea gameArea)
        {
            var startX = (int) camera.Bounds.Left / gameArea.GameWorld.TileSize;
            var startY =(int) camera.Bounds.Top / gameArea.GameWorld.TileSize;
            var endX = (int)camera.Bounds.Right / gameArea.GameWorld.TileSize;
            var endY = (int)camera.Bounds.Bottom / gameArea.GameWorld.TileSize;

            for (float lightChange = 0; lightChange <= 1f; lightChange += 0.1f)
            {
                for (var tileX = startX; tileX <= endX; tileX++)
                {
                    for (var tileY = startY; tileY <= endY; tileY++)
                    {
                        if (IsOutOfBounds(tileX, tileY)) continue;
                        if (Math.Abs(_lightValues[tileX, tileY] - lightChange) > 0.01f) continue;

                        CompareWithNeighbours(tileX, tileY, lightChange);
                    }
                }
            }
        }

        private void CompareWithNeighbours(int tileX, int tileY, float lightChange)
        {
            for (var neighbourX = tileX - 1; neighbourX <= tileX + 1; neighbourX++)
            {
                for (var neighbourY = tileY - 1; neighbourY <= tileY + 1; neighbourY++)
                {
                    if (IsSelf(neighbourX, neighbourY, tileX, tileY)) continue;
                    if (IsDiagonal(neighbourX, neighbourY, tileX, tileY)) continue;
                    if (IsOutOfBounds(neighbourX, neighbourY)) continue;
                    if (LightIsBrighterThanChange(lightChange, neighbourX, neighbourY)) continue;

                    _lightValues[neighbourX, neighbourY] = MathHelper.Clamp( lightChange + 0.1f, 0f, 1f);
                }
            }
        }

        private bool LightIsBrighterThanChange(float lightChange, int neighbourX, int neighbourY) =>
            _lightValues[neighbourX, neighbourY] <= lightChange;

        private static bool IsSelf(int nX, int nY, int lightX, int lightY) =>
            nX == lightX && nY == lightY;

        private bool IsOutOfBounds(int nX, int nY) =>
            nX < 0 || nX >= _lightValues.GetLength(0) || nY < 0 ||
            nY >= _lightValues.GetLength(1);

        private static bool IsDiagonal(int nX, int nY, int lightX, int lightY) =>
            nX == lightX - 1 && nY == lightY - 1 ||
            nX == lightX + 1 && nY == lightY + 1 ||
            nX == lightX + 1 && nY == lightY - 1 ||
            nX == lightX - 1 && nY == lightY + 1;

        public float[,] GetLights()
        {
            ChangedSinceLastGet = false;
            return _lightValues;
        }

        public void Recalculate(Camera camera, IGameArea gameArea) =>
            Task.Run(() => CheckLights(camera, gameArea));
    }
}