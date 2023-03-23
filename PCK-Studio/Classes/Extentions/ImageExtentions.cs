using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Diagnostics;
using System;
using System.Drawing.Imaging;

namespace PckStudio.Classes.Extentions
{
	internal static class ImageExtentions
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
            public Size SectionSize;
            public Point SectionPoint;

            public ImageLayoutInfo(int width, int height, int index, ImageLayoutDirection layoutDirection)
            {
                bool horizontal = layoutDirection == ImageLayoutDirection.Horizontal;
                SectionSize = horizontal ? new Size(width, width) : new Size(height, height);
                SectionPoint = horizontal ? new Point(0, index * width) : new Point(index * height, 0);
            }
        }

        private static Image GetTileImage(Image source, Rectangle area, Size size, GraphicsUnit unit = GraphicsUnit.Pixel)
        {
            Image tileImage = new Bitmap(size.Width, size.Height);
            using (Graphics gfx = Graphics.FromImage(tileImage))
            {
                gfx.SmoothingMode = SmoothingMode.None;
                gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.DrawImage(source, new Rectangle(Point.Empty, size), area, unit);
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
                yield return GetTileImage(source, tileArea, size);
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
                Rectangle tileArea = new Rectangle(locationInfo.SectionPoint, locationInfo.SectionSize);
                yield return GetTileImage(source, tileArea, locationInfo.SectionSize);
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

            // TODO: Validate all source images to be the same size.
            int width = sources[0].Width;
            int heigh = sources[0].Height;

            if (horizontal)
                width *= sources.Length;
            else
                heigh *= sources.Length;

            return new Size(width, heigh);
        }

        public static Image ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}
