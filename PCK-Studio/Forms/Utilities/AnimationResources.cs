using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;

using PckStudio.Properties;
using PckStudio.Extensions;

namespace PckStudio.Forms.Utilities
{
    public static class AnimationResources
    {
        public static string GetAnimationSection(bool isItem) => isItem ? "items" : "blocks";

        public static readonly JObject tileData = JObject.Parse(Resources.tileData);
        private static Image[] _tileImages;
        public static Image[] tileImages => _tileImages ??= Resources.terrain_sheet.CreateImageList(16).Concat(Resources.items_sheet.CreateImageList(16)).ToArray();
    }
}
