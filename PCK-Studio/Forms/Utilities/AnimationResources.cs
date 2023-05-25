using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

using PckStudio.Properties;
using PckStudio.Extensions;
using PckStudio.Forms.Editor;

namespace PckStudio.Forms.Utilities
{
    public static class AnimationResources
    {
        private const string __blocks = "blocks";
        private const string __items = "items";

        internal static string GetAnimationSection(Animation.AnimationCategory category)
        {
            return category switch
            {
                Animation.AnimationCategory.Items => __items,
                Animation.AnimationCategory.Blocks => __blocks,
                _ => throw new ArgumentOutOfRangeException(category.ToString())
            };
        }

        private static JObject _jsonData = JObject.Parse(Resources.tileData);
        public static JObject JsonTileData => _jsonData ??= JObject.Parse(Resources.tileData);
        
        private static Image[] _itemImages;
        public static Image[] ItemImages => _itemImages ??= Resources.items_sheet.CreateImageList(16).ToArray();

        private static Image[] _blockImages;
        public static Image[] BlockImages => _blockImages ??= Resources.terrain_sheet.CreateImageList(16).ToArray();

        private static ImageList _itemList;
        public static ImageList ItemList
        {
            get
            {
                if (_itemList is null)
                {
                    _itemList = new ImageList();
                    _itemList.ColorDepth = ColorDepth.Depth32Bit;
                    _itemList.Images.AddRange(ItemImages);
                }
                return _itemList;
            }
        }

        private static ImageList _blockList;
        public static ImageList BlockList
        {
            get
            {
                if (_blockList is null)
                {
                    _blockList = new ImageList();
                    _blockList.ColorDepth = ColorDepth.Depth32Bit;
                    _blockList.Images.AddRange(BlockImages);
                }
                return _blockList;
            }
        }

        internal static JObject ConvertAnimationToJson(Animation animation, bool interpolation)
        {
            JObject janimation = new JObject();
            JObject mcmeta = new JObject();
            mcmeta["comment"] = $"Animation converted by {Application.ProductName}";
            mcmeta["animation"] = janimation;
            JArray jframes = new JArray();
            foreach (var frame in animation.GetFrames())
            {
                JObject jframe = new JObject();
                jframe["index"] = animation.GetTextureIndex(frame.Texture);
                jframe["time"] = frame.Ticks;
                jframes.Add(jframe);
            };
            janimation["interpolation"] = interpolation;
            janimation["frames"] = jframes;
            return mcmeta;
        }
    }
}
