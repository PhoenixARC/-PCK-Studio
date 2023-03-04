using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Diagnostics;

namespace PckStudio.Classes.Extentions
{
	internal static class ImageExtentions
	{
        public static IEnumerable<Image> CreateImageList(this Image source, int size)
        {
            int img_row_count = source.Width / size;
            int img_column_count = source.Height / size;
            Debug.WriteLine($"{source.Width} {source.Height} {size} {size} {img_column_count} {img_row_count}");
            for (int i = 0; i < img_column_count * img_row_count; i++)
            {
                int row = i / img_row_count;
                int column = i % img_row_count;
                Rectangle tileArea = new Rectangle(new Point(column * size, row * size), new Size(size, size));
                Bitmap tileImage = new Bitmap(size, size);
                using (Graphics gfx = Graphics.FromImage(tileImage))
                {
                    gfx.SmoothingMode = SmoothingMode.None;
                    gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    gfx.DrawImage(source, new Rectangle(0, 0, size, size), tileArea, GraphicsUnit.Pixel);
                }
                yield return tileImage;
            }
            yield break;
        }
    }
}
