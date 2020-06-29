using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mayday.Game.Enums;
using Mayday.Game.Gameplay.Entities;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Lighting
{
    public class LightMap
    {

        private float[,] _lightValues;

        public async Task<float[,]> CheckLights(IEntity player, Camera camera)
        {
            var gameArea = player.GameArea;

            _lightValues = new float[gameArea.AreaWidth, gameArea.AreaHeight];
            var ignores = new List<Vector2>();

            var xStart = (int) camera.Bounds.Left / player.GameWorld.TileSize - 10;
            var xEnd =(int) camera.Bounds.Right / player.GameWorld.TileSize + 10;
            var yStart = (int)camera.Bounds.Top / player.GameWorld.TileSize - 10;
            var yEnd = (int)camera.Bounds.Bottom / player.GameWorld.TileSize + 10;

            xStart = 0;
            yStart = 0;
            xEnd = _lightValues.GetLength(0);
            yEnd = _lightValues.GetLength(1);

            for (var i = xStart; i <= xEnd; i++)
            {
                for (var j = yStart; j <= yEnd; j++)
                {
                    if (i < 0 || i >= _lightValues.GetLength(0)) continue;
                    if(j < 0 || j >= _lightValues.GetLength(1)) continue;

                    if (ignores.Contains(new Vector2(j, i)))
                    {
                        continue;
                    }

                    var tile = gameArea.TryGetTile(i, j);

                    if (tile == null)
                    {
                        _lightValues[i, j] = 1;
                        continue;
                    }

                    if (tile.TileType == TileTypes.None)
                    {
                        _lightValues[i, j] = 0;
                    }
                    else
                    {
                        _lightValues[i, j] = 1f;
                    }
                }
            }

            for (float i = 0; i <= 1f; i += 0.1f)
            {
                for (var lightX = xStart; lightX <= xEnd; lightX++)
                {
                    for (var lightY = yStart; lightY <= yEnd; lightY++)
                    {
                        if (lightX < 0 || lightX >= _lightValues.GetLength(0)) continue;
                        if(lightY < 0 || lightY >= _lightValues.GetLength(1)) continue;

                        if (Math.Abs(_lightValues[lightX, lightY] - i) > 0.01f) continue;

                        for (var nX = lightX - 1; nX <= lightX + 1; nX++)
                        {
                            for (var nY = lightY - 1; nY <= lightY + 1; nY++)
                            {
                                if (nX == lightX && nY == lightY)
                                {
                                    continue;
                                }

                                if ((nX == lightX - 1 && nY == lightY - 1) ||
                                    (nX == lightX + 1 && nY == lightY + 1) ||
                                    (nX == lightX + 1 && nY == lightY - 1) ||
                                    (nX == lightX - 1 && nY == lightY + 1))
                                {
                                    continue;
                                }

                                if (nX < 0 || nX >= _lightValues.GetLength(0) - 1 || nY < 0 || nY >= _lightValues.GetLength(1) - 1) continue;
                                if (Math.Abs(_lightValues[nX, nY] - i) < 0.01f || !(i < _lightValues[nX, nY])) continue;

                                _lightValues[nX, nY] = i + (0.1f);
                            }
                        }
                    }
                }
            }

            return await Task.FromResult(_lightValues);
        }
    }

}