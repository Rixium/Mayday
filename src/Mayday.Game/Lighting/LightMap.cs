using System;
using System.Threading.Tasks;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.World.Areas;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Lighting
{
    public class LightMap
    {

        private IGameArea _gameArea;
        public IGameArea GameArea
        {
            get => _gameArea;
            set
            {
                _lightValues = new float[value.AreaWidth, value.AreaHeight];
                _xEnd = _lightValues.GetLength(0);
                _yEnd = _lightValues.GetLength(1);
                _gameArea = value;
            }
        }

        private float[,] _lightValues;
        private int _yEnd;
        private int _xEnd;

        public bool ChangedSinceLastGet { get; set; }

        public async Task<float[,]> CheckLights(IGameArea gameArea)
        {
            GameArea = gameArea;

            PreCalculateLightValues();
            CalculateLighting();

            ChangedSinceLastGet = true;

            return await Task.FromResult(_lightValues);
        }

        private void PreCalculateLightValues()
        {
            for (var tileX = 0; tileX <= _xEnd; tileX++)
            {
                for (var tileY = 0; tileY <= _yEnd; tileY++)
                {
                    if (tileX < 0 || tileX >= _lightValues.GetLength(0)) continue;
                    if (tileY < 0 || tileY >= _lightValues.GetLength(1)) continue;

                    var tile = GameArea.TryGetTile(tileX, tileY);

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

        private void CalculateLighting()
        {
            for (float lightChange = 0; lightChange <= 1f; lightChange += 0.1f)
            {
                for (var tileX = 0; tileX <= _xEnd; tileX++)
                {
                    for (var tileY = 0; tileY <= _yEnd; tileY++)
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
    }
}