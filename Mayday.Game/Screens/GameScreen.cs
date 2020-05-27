using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Mayday.Game.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using Yetiface.Engine.Screens;
using Yetiface.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;

namespace Mayday.Game.Screens
{
    public class GameScreen : Screen
    {
        
        private IWorld _world;

        private Texture2D _texture;
        public Bitmap Bitmap;
        
        public GameScreen() : base("GameScreen")
        {
        }

        public void SetWorld(IWorld world)
        {
            _world = world;
        }

        public override void Awake()
        {
            _texture = GetTexture(GraphicsUtils.Instance.SpriteBatch.GraphicsDevice, Bitmap);
        }
        
        private Texture2D GetTexture(GraphicsDevice dev, Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(dev, bmp.Width, bmp.Height);

            unsafe
            {
                // lock bitmap
                BitmapData origdata = 
                    bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                uint* byteData = (uint*)origdata.Scan0;

                // Switch bgra -> rgba
                for (int i = 0; i < imgData.Length; i++)
                {
                    byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);                        
                }                

                // copy data
                Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                // unlock bitmap
                bmp.UnlockBits(origdata);
            }

            texture.SetData(imgData);

            return texture;
        }

        public override void Begin()
        {
        }

        public override void Draw()
        {
            base.Draw();
            
            GraphicsUtils.Instance.Begin(true);

            GraphicsUtils.Instance.SpriteBatch.Draw(_texture, new Microsoft.Xna.Framework.Rectangle(0, 0, Window.ViewportWidth, Window.ViewportHeight), Color.White);
            
            GraphicsUtils.Instance.End();
        }

        public override void Finish()
        {
        }
    }
}