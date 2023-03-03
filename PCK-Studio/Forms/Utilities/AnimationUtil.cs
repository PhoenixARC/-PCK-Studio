using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using System.Drawing.Imaging;
using PckStudio.Classes.Utils;
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
                    _tileImages = ImageUtils.CreateImageList(Resources.terrain_sheet, 16).Concat(ImageUtils.CreateImageList(Resources.items_sheet, 16)).ToArray();
                return _tileImages;
            }
        }
        public static PCKFile.FileData CreateNewAnimationFile(Image source, string tileName, bool isItem)
        {
            PCKFile.FileData file = new PCKFile.FileData($"res/textures/{GetAnimationSection(isItem)}/{tileName}.png", PCKFile.FileData.FileType.TextureFile);
            file.Properties.Add(("ANIM", string.Empty));
            using (var stream = new MemoryStream())
            {
                source.Save(stream, ImageFormat.Png);
                file.SetData(stream.ToArray());
            }
            return file;
        }
    }
}
