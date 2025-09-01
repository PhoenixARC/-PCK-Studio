/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PckStudio.Core.Extensions
{
    public static class ImageExtensions
    {
        public static Image ReleaseFromFile(this Image image)
        {
            Image img = new Bitmap(image);
            image.Dispose();
            return img;
        }

        public static Image GetArea(this Image source, Rectangle area)
        {
            Image tileImage = new Bitmap(area.Width, area.Height);
            using (Graphics gfx = Graphics.FromImage(tileImage))
            {
                gfx.SmoothingMode = SmoothingMode.None;
                gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.DrawImage(source, new Rectangle(Point.Empty, area.Size), area, GraphicsUnit.Pixel);
            }
            return tileImage;
        }

        /// <summary>
        /// Creates an IEnumerable by reading in horizontal order
        /// </summary>
        /// <param name="source">this image</param>
        /// <param name="scalar">Indecates width and height of image sub section</param>
        /// <returns><see cref="IEnumerable{Image}"/> of type <see cref="Image"/></returns>
        public static IEnumerable<Image> SplitHorizontal(this Image source, int scalar)
        {
            return source.Split(scalar, ImageLayoutDirection.Horizontal);
        }

        public static IEnumerable<Image> Split(this Image source, int scalar, ImageLayoutDirection layoutDirection)
        {
            return Split(source, new Size(scalar, scalar), layoutDirection);
        }

        public static IEnumerable<Image> Split(this Image source, Size size, ImageLayoutDirection imageLayout)
        {
            int rowCount = source.Width / size.Width;
            int columnCount = source.Height / size.Height;
            Debug.WriteLine($"Image size: {source.Size}, Area size: {size}, col num: {columnCount}, row num: {rowCount}");
            for (int i = 0; i < columnCount * rowCount; i++)
            {
                int row = Math.DivRem(i, rowCount, out int column);
                if (imageLayout == ImageLayoutDirection.Vertical)
                    column = Math.DivRem(i, columnCount, out row);
                Rectangle tileArea = new Rectangle(new Point(column * size.Width, row * size.Height), size);
                yield return source.GetArea(tileArea);
            }
            yield break;
        }

        public static IEnumerable<Image> Split(this Image source, ImageLayoutDirection layoutDirection)
        {
            for (int i = 0; i < source.Height / source.Width; i++)
            {
                ImageSection locationInfo = new ImageSection(source.Size, i, layoutDirection);
                yield return source.GetArea(locationInfo.Area);
            }
            yield break;
        }

        public static Image Combine(this IEnumerable<Image> sources, ImageLayoutDirection layoutDirection)
        {
            Size imageSize = CalculateImageSize(sources, layoutDirection);
            var image = new Bitmap(imageSize.Width, imageSize.Height);

            using (var graphic = Graphics.FromImage(image))
            {
                foreach ((int i, Image texture) in sources.enumerate())
                {
                    var info = new ImageSection(texture.Size, i, layoutDirection);
                    graphic.DrawImage(texture, info.Point);
                }
            }
            return image;
        }

        private static Size CalculateImageSize(IEnumerable<Image> sources, ImageLayoutDirection layoutDirection)
        {
            Size size = sources.First().Size;
            int count = sources.Count();

            if (count < 2)
                return count < 1 ? Size.Empty : size;

            var horizontal = layoutDirection == ImageLayoutDirection.Horizontal;

            if (!sources.All(img => img.Size == size))
                throw new InvalidOperationException("Images must have the same width and height.");

            if (horizontal)
                size.Width *= count;
            else
                size.Height *= count;

            return size;
        }

        public static Image Resize(this Image image, Size size, GraphicsConfig graphicsConfig)
        {
            return image.Resize(size.Width, size.Height, graphicsConfig);
        }

        public static Image Resize(this Image image, int width, int height, GraphicsConfig graphicsConfig)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.ApplyConfig(graphicsConfig);
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public static Image Blend(this Image image, Color overlayColor, BlendMode mode)
        {
            if (image is not Bitmap baseImage)
                return image;

            Bitmap bitmapResult = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);

            BitmapData baseImageData = baseImage.LockBits(new Rectangle(Point.Empty, baseImage.Size),
                        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData resultImageData = bitmapResult.LockBits(new Rectangle(Point.Empty, bitmapResult.Size),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Parallel.For(0, baseImageData.Stride * baseImageData.Height / 4, (i) =>
            {
                int k = i * 4;
                unsafe
                {
                    int color = Unsafe.Read<int>((baseImageData.Scan0 + k).ToPointer());
                    byte a = (byte)(color >> 24 & 0xff);
                    if (a == 0)
                    {
                        Unsafe.Write((resultImageData.Scan0 + k).ToPointer(), 0);
                        return;
                    }
                    var b = ColorExtensions.BlendValues((byte)(color >> 0 & 0xff), overlayColor.B, mode);
                    var g = ColorExtensions.BlendValues((byte)(color >> 8 & 0xff), overlayColor.G, mode);
                    var r = ColorExtensions.BlendValues((byte)(color >> 16 & 0xff), overlayColor.R, mode);
                    int blendedValue = a << 24 | r << 16 | g << 8 | b;
                    Unsafe.Write((resultImageData.Scan0 + k).ToPointer(), blendedValue);
                }
            });

            bitmapResult.UnlockBits(resultImageData);
            baseImage.UnlockBits(baseImageData);
            return bitmapResult;
        }

        public static Image Blend(this Image image, Image overlay, BlendMode mode)
        {
            if (image is not Bitmap baseImage || overlay is not Bitmap overlayImage ||
                image.Width != overlay.Width || image.Height != overlay.Height)
                return image;

            BitmapData baseImageData = baseImage.LockBits(new Rectangle(Point.Empty, baseImage.Size),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] baseImageBuffer = new byte[baseImageData.Stride * baseImageData.Height];

            Marshal.Copy(baseImageData.Scan0, baseImageBuffer, 0, baseImageBuffer.Length);

            BitmapData overlayImageData = overlayImage.LockBits(new Rectangle(Point.Empty, overlayImage.Size), 
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] overlayImageBuffer = new byte[overlayImageData.Stride * overlayImageData.Height];

            Marshal.Copy(overlayImageData.Scan0, overlayImageBuffer, 0, overlayImageBuffer.Length);

            for (int k = 0; k < baseImageBuffer.Length && k < overlayImageBuffer.Length; k += 4)
            {
                baseImageBuffer[k + 0] = ColorExtensions.BlendValues(baseImageBuffer[k + 0], overlayImageBuffer[k + 0], mode);
                baseImageBuffer[k + 1] = ColorExtensions.BlendValues(baseImageBuffer[k + 1], overlayImageBuffer[k + 1], mode);
                baseImageBuffer[k + 2] = ColorExtensions.BlendValues(baseImageBuffer[k + 2], overlayImageBuffer[k + 2], mode);
            }

            Bitmap bitmapResult = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);
            BitmapData resultImageData = bitmapResult.LockBits(new Rectangle(Point.Empty, bitmapResult.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(baseImageBuffer, 0, resultImageData.Scan0, baseImageBuffer.Length);

            bitmapResult.UnlockBits(resultImageData);
            baseImage.UnlockBits(baseImageData);
            overlayImage.UnlockBits(overlayImageData);
            return bitmapResult;
        }

        public static Image Interpolate(this Image image1, Image image2, double delta)
        {
            delta = MathExtensions.Clamp(delta, 0.0, 1.0);
            if (image1 is not Bitmap baseImage || image2 is not Bitmap overlayImage ||
                image1.Width != image2.Width || image1.Height != image2.Height)
                return image1;

            BitmapData baseImageData = baseImage.LockBits(new Rectangle(Point.Empty, baseImage.Size),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] baseImageBuffer = new byte[baseImageData.Stride * baseImageData.Height];

            Marshal.Copy(baseImageData.Scan0, baseImageBuffer, 0, baseImageBuffer.Length);

            baseImage.UnlockBits(baseImageData);

            BitmapData overlayImageData = overlayImage.LockBits(new Rectangle(Point.Empty, overlayImage.Size),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] overlayImageBuffer = new byte[overlayImageData.Stride * overlayImageData.Height];
            
            Marshal.Copy(overlayImageData.Scan0, overlayImageBuffer, 0, overlayImageBuffer.Length);

            overlayImage.UnlockBits(overlayImageData);

            byte[] finalBuffer = new byte[baseImageData.Stride * baseImageData.Height];
            for (int k = 0; k < baseImageBuffer.Length && k < overlayImageBuffer.Length; k += 4)
            {
                finalBuffer[k + 0] = ColorExtensions.Mix(delta, baseImageBuffer[k + 0], overlayImageBuffer[k + 0]);
                finalBuffer[k + 1] = ColorExtensions.Mix(delta, baseImageBuffer[k + 1], overlayImageBuffer[k + 1]);
                finalBuffer[k + 2] = ColorExtensions.Mix(delta, baseImageBuffer[k + 2], overlayImageBuffer[k + 2]);
                finalBuffer[k + 3] = ColorExtensions.Mix(delta, baseImageBuffer[k + 3], overlayImageBuffer[k + 3]);
            }

            Bitmap bitmapResult = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);
            BitmapData resultImageData = bitmapResult.LockBits(new Rectangle(Point.Empty, bitmapResult.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(finalBuffer, 0, resultImageData.Scan0, finalBuffer.Length);

            bitmapResult.UnlockBits(resultImageData);
            return bitmapResult;
        }
    }
}
