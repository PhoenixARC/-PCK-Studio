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
    internal static class ImageExtensions
    {
        public enum ImageLayoutDirection
        {
            Horizontal,
            Vertical
        }

        private struct ImageLayoutInfo
        {
            /// <summary>
            /// Size of sub section of the image
            /// </summary>
            public readonly Size SectionSize;
            public readonly Point SectionPoint;
            public readonly Rectangle SectionArea;

            public ImageLayoutInfo(int width, int height, int index, ImageLayoutDirection layoutDirection)
            {
                bool horizontal = layoutDirection == ImageLayoutDirection.Horizontal;
                SectionSize = horizontal ? new Size(width, width) : new Size(height, height);
                SectionPoint = horizontal ? new Point(0, index * width) : new Point(index * height, 0);
                SectionArea = new Rectangle(SectionPoint, SectionSize);
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
            int img_row_count = source.Width / size.Width;
            int img_column_count = source.Height / size.Height;
            Debug.WriteLine($"{source.Width} {source.Height} {size} {img_column_count} {img_row_count}");
            for (int i = 0; i < img_column_count * img_row_count; i++)
            {
                int row = i / img_row_count;
                int column = i % img_row_count;
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
                ImageLayoutInfo locationInfo = new ImageLayoutInfo(source.Width, source.Height, i, layoutDirection);
                yield return source.GetArea(locationInfo.SectionArea);
            }
            yield break;
        }

        public static Image ImageFromImageArray(Image[] sources, ImageLayoutDirection layoutDirection)
        {
            Size imageSize = CalculateImageSize(sources, layoutDirection);
            var result = new Bitmap(imageSize.Width, imageSize.Height);

            using (var graphic = Graphics.FromImage(result))
            {
                foreach (var (i, texture) in sources.enumerate())
                {
                    var info = new ImageLayoutInfo(imageSize.Width, imageSize.Height, i, layoutDirection);
                    graphic.DrawImage(texture, info.SectionPoint);
                };
            }
            return result;
        }

        private static Size CalculateImageSize(Image[] sources, ImageLayoutDirection layoutDirection)
        {
            var horizontal = layoutDirection == ImageLayoutDirection.Horizontal;

            int width = sources[0].Width;
            int height = sources[0].Height;
            if (!sources.All(img => img.Width.Equals(width) && img.Height.Equals(height)))
                throw new InvalidOperationException("Images must have the same width and height.");

            if (horizontal)
                width *= sources.Length;
            else
                height *= sources.Length;

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

        public static Image Blend(this Image bg, Color foregroundColor, BlendMode mode)
        {
            Bitmap bitmap = new Bitmap(bg);
            bitmap.Fill(foregroundColor);
            return bg.Blend(bitmap, mode);
        }

        public static Image Blend(this Image image, Image overlay, BlendMode mode)
        {
            if (image is not Bitmap baseImage || overlay is not Bitmap overlayImage)
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
                baseImageBuffer[k + 0] = CalculateColorComponentBlendValue(baseImageBuffer[k + 0] / 255f, overlayImageBuffer[k + 0] / 255f, mode);
                baseImageBuffer[k + 1] = CalculateColorComponentBlendValue(baseImageBuffer[k + 1] / 255f, overlayImageBuffer[k + 1] / 255f, mode);
                baseImageBuffer[k + 2] = CalculateColorComponentBlendValue(baseImageBuffer[k + 2] / 255f, overlayImageBuffer[k + 2] / 255f, mode);
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

        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            else if (value.CompareTo(max) > 0) return max;
            else return value;
        }

        private static byte CalculateColorComponentBlendValue(float source, float overlay, BlendMode blendType)
        {
            source = Clamp(source, 0.0f, 1.0f);
            overlay = Clamp(overlay, 0.0f, 1.0f);

            float resultValue = blendType switch
            {
                BlendMode.Add => source + overlay,
                BlendMode.Subtract => source - overlay,
                BlendMode.Multiply => source * overlay,
                BlendMode.Average => (source + overlay) / 2.0f,
                BlendMode.AscendingOrder => source > overlay ? overlay : source,
                BlendMode.DescendingOrder => source < overlay ? overlay : source,
                BlendMode.Screen => 1f - (1f - source)*(1f - overlay),
                _ => 0.0f
            };
            return (byte)Clamp(resultValue * 255, 0, 255);
        }
    }
}
