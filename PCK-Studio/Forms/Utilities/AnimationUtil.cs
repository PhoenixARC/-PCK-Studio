using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using System.Drawing.Imaging;
using System.IO;

namespace PckStudio.Forms.Utilities
{
    public static class AnimationUtil
    {
        public static string GetAnimationSection(bool isItem) => isItem ? "items" : "blocks";

        public static readonly JObject tileData = JObject.Parse(Resources.tileData);
        private static Image[] _tileImages;
        public static Image[] tileImages
        {
            get { 
                if (_tileImages == null)
                    _tileImages = CreateImageList(Resources.terrain_sheet, 16, 16).Concat(CreateImageList(Resources.items_sheet, 16, 16)).ToArray();
                return _tileImages;
            }
        }
        public static PCKFile.FileData CreateNewAnimationFile(Image source, string tileName, bool isItem)
        {
            PCKFile.FileData file = new PCKFile.FileData($"res/textures/{GetAnimationSection(isItem)}/{tileName}.png", PCKFile.FileData.FileType.TextureFile);
            file.properties.Add(("ANIM", string.Empty));
            using (var stream = new MemoryStream())
            {
                source.Save(stream, ImageFormat.Png);
                file.SetData(stream.ToArray());
            }
            return file;
        }


        private static IEnumerable<Image> CreateImageList(Image source, int width, int height)
        {
            int img_row_count = source.Width / width;
            int img_column_count = source.Height / height;
            for (int i = 0; i < img_column_count * img_row_count; i++)
            {
                int row = i / width;
                int column = i % height;
                Rectangle tileArea = new Rectangle(new Point(column * width, row * height), new Size(width, height));
                Bitmap tileImage = new Bitmap(width, height);
                using (Graphics gfx = Graphics.FromImage(tileImage))
                {
                    gfx.SmoothingMode = SmoothingMode.None;
                    gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    gfx.DrawImage(source, new Rectangle(0, 0, width, height), tileArea, GraphicsUnit.Pixel);
                }
                yield return tileImage;
            }
            yield break;
        }

    }
}
