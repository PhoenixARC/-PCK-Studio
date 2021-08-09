using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace stonevox
{
    public static class Screenshot
    {
        public static Bitmap ScreenShot()
        {
            return ScreenShot(ReadBufferMode.Front);
        }

        public static Bitmap ScreenShot(ReadBufferMode buffer)
        {
            Bitmap transfermap = new Bitmap(Client.window.Width, Client.window.Height);
            System.Drawing.Imaging.BitmapData data =
               transfermap.LockBits(new Rectangle(0, 0, transfermap.Width, transfermap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.ReadBuffer(buffer);
            GL.ReadPixels(0, 0, transfermap.Width, transfermap.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

            unsafe
            {
                int PixelSize = 4;
                unsafe
                {
                    for (int y = 0; y < data.Height; y++)
                    {
                        byte* row = (byte*)data.Scan0 + (y * data.Stride);

                        for (int x = 0; x < data.Width; x++)
                        {
                            byte r = row[x * PixelSize + 2];
                            byte b = row[x * PixelSize];
                            row[x * PixelSize] = r;
                            row[x * PixelSize + 2] = b;
                        }
                    }
                }
            }

            transfermap.UnlockBits(data);
            transfermap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return transfermap;
        }

        public static Bitmap ScreenShot(int xx, int yy, int width, int height, ReadBufferMode buffer)
        {
            Bitmap transfermap = new Bitmap(width, height);
            System.Drawing.Imaging.BitmapData data =
               transfermap.LockBits(new Rectangle(0, 0, transfermap.Width, transfermap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.ReadBuffer(buffer);
            GL.ReadPixels(xx, yy, transfermap.Width, transfermap.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

            unsafe
            {
                int PixelSize = 4;
                unsafe
                {
                    for (int y = 0; y < data.Height; y++)
                    {
                        byte* row = (byte*)data.Scan0 + (y * data.Stride);

                        for (int x = 0; x < data.Width; x++)
                        {
                            byte r = row[x * PixelSize + 2];
                            byte b = row[x * PixelSize];
                            row[x * PixelSize] = r;
                            row[x * PixelSize + 2] = b;
                        }
                    }
                }
            }

            transfermap.UnlockBits(data);
            transfermap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return transfermap;
        }

        private static Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
    }
}
