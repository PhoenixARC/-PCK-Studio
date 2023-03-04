using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using System.Drawing.Imaging;
using PckStudio.Classes.Extentions;

namespace PckStudio.Forms.Utilities
{
    public static class AnimationUtil
    {
        public static string GetAnimationSection(bool isItem) => isItem ? "items" : "blocks";

        public static readonly JObject tileData = JObject.Parse(Resources.tileData);
        private static Image[] _tileImages;

        public static Image[] tileImages => _tileImages ??= Resources.terrain_sheet.CreateImageList(16).Concat(Resources.items_sheet.CreateImageList(16)).ToArray();

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
