using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Internal
{
    internal class ResourceLocation
    {
        public static string GetPathFromCategory(ResourceCategory category)
        {
            return category switch
            {
                ResourceCategory.ItemAnimation => "res/textures/items",
                ResourceCategory.BlockAnimation => "res/textures/blocks",
                _ => string.Empty
            };
        }

        public static ResourceCategory GetCategoryFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("res/"))
                return ResourceCategory.Unknown;

            if (path.StartsWith("res/textures/items"))
                return ResourceCategory.ItemAnimation;

            if (path.StartsWith("res/textures/blocks"))
                return ResourceCategory.BlockAnimation;

            return ResourceCategory.Unknown;
        }
    }
}
