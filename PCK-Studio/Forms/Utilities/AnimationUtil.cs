using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using System.Drawing.Imaging;
using PckStudio.Classes.Extentions;
using OMI.Formats.Pck;

namespace PckStudio.Forms.Utilities
{
    public static class AnimationUtil
    {
        public static string GetAnimationSection(bool isItem) => isItem ? "items" : "blocks";

        public static readonly JObject tileData = JObject.Parse(Resources.tileData);
        private static Image[] _tileImages;

        public static Image[] tileImages => _tileImages ??= Resources.terrain_sheet.CreateImageList(16).Concat(Resources.items_sheet.CreateImageList(16)).ToArray();

        public static PckFile.FileData CreateNewAnimationFile(Image source, string tileName, bool isItem)
        {
            PckFile.FileData file = new PckFile.FileData($"res/textures/{GetAnimationSection(isItem)}/{tileName}.png", PckFile.FileData.FileType.TextureFile);
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
