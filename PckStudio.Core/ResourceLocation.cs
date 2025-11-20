/* Copyright (c) 2024-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using PckStudio.Core.Json;

namespace PckStudio.Core
{
    public class ResourceLocation
    {
        internal const string RESOURCE_PATH_PREFIX = "res/";
        private static List<ResourceLocation> ResourceGroups = new List<ResourceLocation>();
        private static readonly ResourceLocation Unknown = new ResourceLocation(string.Empty, ResourceCategory.Unknown);
        private static readonly Dictionary<string, ResourceLocation> _pathLookUp = new Dictionary<string, ResourceLocation>();
        private static readonly Dictionary<ResourceCategory, ResourceLocation> _categoryLookUp = new Dictionary<ResourceCategory, ResourceLocation>();

        public string Path { get; }
        public string FullPath => System.IO.Path.Combine(RESOURCE_PATH_PREFIX, Path);
        public ResourceCategory Category { get; }
        public bool IsGroup { get; }

        protected ResourceLocation(string path, ResourceCategory category, bool isGroup = false)
        {
            Path = path;
            Category = category;
            IsGroup = isGroup;
         
            if (IsGroup)
                ResourceGroups.Add(this);

            if (Category != ResourceCategory.Unknown && !string.IsNullOrWhiteSpace(Path))
            {
                _categoryLookUp.Add(Category, this);
                _pathLookUp.Add(Path, this);
                Debug.WriteLine($"Add ResourceLocation: {Path}({Category}).");
            }
        }

        public override string ToString() => FullPath;

        internal static ResourceLocation GetFromCategory(ResourceCategory category) => _categoryLookUp.ContainsKey(category) ? _categoryLookUp[category] : Unknown;

        internal static string GetPathFromCategory(ResourceCategory category) => GetFromCategory(category).ToString();

        internal static ResourceCategory GetCategoryFromPath(string path) => GetFromPath(path).Category;

        internal static ResourceLocation GetFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith(RESOURCE_PATH_PREFIX))
                return Unknown;
            string categoryPath = path.Substring(RESOURCE_PATH_PREFIX.Length);
            if (_pathLookUp.ContainsKey(categoryPath))
                return _pathLookUp[categoryPath];
            return ResourceGroups.Where(group => categoryPath.StartsWith(group.Path)).FirstOrDefault() ?? Unknown;
        }
    }
}
