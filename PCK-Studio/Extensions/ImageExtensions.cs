using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace PckStudio.Extensions
{
    public enum ImageLayoutDirection
    {
        Horizontal,
        Vertical
    }

    internal static class ImageExtensions
    {
        private struct ImageSection
        {
            public readonly Size Size;
            public readonly Point Point;
            public readonly Rectangle Area;

            public ImageSection(Size sectionSize, int index, ImageLayoutDirection layoutDirection)
            {
                switch(layoutDirection)
                {
                    case ImageLayoutDirection.Horizontal:
                        {
                            Size = new Size(sectionSize.Height, sectionSize.Height);
                            Point = new Point(index * sectionSize.Height, 0);
                        }
                        break;

                    case ImageLayoutDirection.Vertical:
                        {
                            Size = new Size(sectionSize.Width, sectionSize.Width);
                            Point = new Point(0, index * sectionSize.Width);
                        }
                        break;

                    default:
                        Size = Size.Empty;
                        Point = new Point(-1, -1);
                        break;
                }
                Area = new Rectangle(Point, Size);
            }
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

        public static IEnumerable<Image> CreateImageList(this Image source, Size size)
        {
            int rowCount = source.Width / size.Width;
            int columnCount = source.Height / size.Height;
            Debug.WriteLine($"{source.Width} {source.Height} {size} {columnCount} {rowCount}");
            for (int i = 0; i < columnCount * rowCount; i++)
            {
                int row = Math.DivRem(i, rowCount, out int column);
                Rectangle tileArea = new Rectangle(new Point(column * size.Height, row * size.Width), size);
                yield return source.GetArea(tileArea);
            }
            yield break;
        }

        public static IEnumerable<Image> CreateImageList(this Image source, int scalar)
        {
            return CreateImageList(source, new Size(scalar, scalar));
        }

        public static IEnumerable<Image> CreateImageList(this Image source, ImageLayoutDirection layoutDirection)
        {
            for (int i = 0; i < source.Height / source.Width; i++)
            {
                ImageSection locationInfo = new ImageSection(source.Size, i, layoutDirection);
                yield return source.GetArea(locationInfo.Area);
            }
            yield break;
        }

        public static Image CombineImages(IList<Image> sources, ImageLayoutDirection layoutDirection)
        {
            Size imageSize = CalculateImageSize(sources, layoutDirection);
            var image = new Bitmap(imageSize.Width, imageSize.Height);

            using (var graphic = Graphics.FromImage(image))
            {
                foreach (var (i, texture) in sources.enumerate())
                {
                    var info = new ImageSection(texture.Size, i, layoutDirection);
                    graphic.DrawImage(texture, info.Point);
                };
            }
            return image;
        }

        private static Size CalculateImageSize(IList<Image> sources, ImageLayoutDirection layoutDirection)
        {
            if (sources.Count == 0)
            {
                return Size.Empty;
            }
            var horizontal = layoutDirection == ImageLayoutDirection.Horizontal;

            int width = sources[0].Width;
            int height = sources[0].Height;

            if (!sources.All(img => img.Width.Equals(width) && img.Height.Equals(height)))
                throw new InvalidOperationException("Images must have the same width and height.");

            if (horizontal)
                width *= sources.Count;
            else
                height *= sources.Count;

            return new Size(width, height);
        }

        public static Image ResizeImage(this Image image, int width, int height, GraphicsConfig graphicsConfig)
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

        public static Image Fill(this Image image, Color color)
        {
            using (var g = Graphics.FromImage(image))
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, new Rectangle(Point.Empty, image.Size));
                }
            }
            return image;
        }

        public static Image Blend(this Image image, Color overlayColor, BlendMode mode)
        {
            if (image is not Bitmap baseImage)
                return image;

            BitmapData baseImageData = baseImage.LockBits(new Rectangle(Point.Empty, baseImage.Size),
                        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] baseImageBuffer = new byte[baseImageData.Stride * baseImageData.Height];

            Marshal.Copy(baseImageData.Scan0, baseImageBuffer, 0, baseImageBuffer.Length);
            
            var normalized = overlayColor.Normalize();

            for (int k = 0; k < baseImageBuffer.Length; k += 4)
            {
                baseImageBuffer[k + 0] = ColorExtensions.BlendValues(baseImageBuffer[k + 0] / 255f, normalized.X, mode);
                baseImageBuffer[k + 1] = ColorExtensions.BlendValues(baseImageBuffer[k + 1] / 255f, normalized.Y, mode);
                baseImageBuffer[k + 2] = ColorExtensions.BlendValues(baseImageBuffer[k + 2] / 255f, normalized.Z, mode);
            }

            Bitmap bitmapResult = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);
            BitmapData resultImageData = bitmapResult.LockBits(new Rectangle(Point.Empty, bitmapResult.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(baseImageBuffer, 0, resultImageData.Scan0, baseImageBuffer.Length);

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
                baseImageBuffer[k + 0] = ColorExtensions.BlendValues(baseImageBuffer[k + 0] / 255f, overlayImageBuffer[k + 0] / 255f, mode);
                baseImageBuffer[k + 1] = ColorExtensions.BlendValues(baseImageBuffer[k + 1] / 255f, overlayImageBuffer[k + 1] / 255f, mode);
                baseImageBuffer[k + 2] = ColorExtensions.BlendValues(baseImageBuffer[k + 2] / 255f, overlayImageBuffer[k + 2] / 255f, mode);
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
    }
}
