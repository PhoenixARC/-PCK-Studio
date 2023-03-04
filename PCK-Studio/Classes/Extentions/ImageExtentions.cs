using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Diagnostics;

namespace PckStudio.Classes.Extentions
{
	internal static class ImageExtentions
	{
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
                Bitmap tileImage = new Bitmap(size.Width, size.Height);
                using (Graphics gfx = Graphics.FromImage(tileImage))
                {
                    gfx.SmoothingMode = SmoothingMode.None;
                    gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    gfx.DrawImage(source, new Rectangle(Point.Empty, size), tileArea, GraphicsUnit.Pixel);
                }
                yield return tileImage;
            }
            yield break;
        }


        public static IEnumerable<Image> CreateImageList(this Image source, int scalar)
        {
            return CreateImageList(source, new Size(scalar, scalar));
        }
    }
}
