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
using System.Drawing;
using System.Linq;

namespace PckStudio.Internal
{
    internal sealed class ResourceLocation
    {
        private static List<ResourceLocation> ResourceGroups = new List<ResourceLocation>();
        private static readonly ResourceLocation Unknown = new ResourceLocation(string.Empty, ResourceCategory.Unknown, -1);

        private static readonly Dictionary<string, ResourceLocation> _categoryLookUp = new Dictionary<string, ResourceLocation>()
        {
            ["textures/items"]               = new ResourceLocation("textures/items", ResourceCategory.ItemAnimation, 16, isGroup: true),
            ["textures/blocks"]              = new ResourceLocation("textures/blocks", ResourceCategory.BlockAnimation, 16, isGroup: true),
            ["mob"]                          = new ResourceLocation("mob", ResourceCategory.MobEntityTextures, 1, isGroup: true),
            ["item"]                         = new ResourceLocation("item", ResourceCategory.ItemEntityTextures, 1, isGroup: true),
            ["terrain.png"]                  = new ResourceLocation("terrain.png", ResourceCategory.BlockAtlas, 16),
            ["items.png"]                    = new ResourceLocation("items.png", ResourceCategory.ItemAtlas, 16),
            ["particles.png"]                = new ResourceLocation("particles.png", ResourceCategory.ParticleAtlas, 16),
            ["item/banner/Banner_Atlas.png"] = new ResourceLocation("item/banner/Banner_Atlas.png", ResourceCategory.BannerAtlas, new Size(6, 7), TillingMode.WidthAndHeight),
            ["art/kz.png"]                   = new ResourceLocation("art/kz.png", ResourceCategory.PaintingAtlas, 16),
            ["misc/explosion.png"]           = new ResourceLocation("misc/explosion.png", ResourceCategory.ExplosionAtlas, 4),
            ["item/xporb.png"]               = new ResourceLocation("item/xporb.png", ResourceCategory.ExperienceOrbAtlas, 4),
            ["terrain/moon_phases.png"]      = new ResourceLocation("terrain/moon_phases.png", ResourceCategory.MoonPhaseAtlas, 4),
            ["misc/mapicons.png"]            = new ResourceLocation("misc/mapicons.png", ResourceCategory.MapIconAtlas, 4),
            ["misc/additionalmapicons.png"]  = new ResourceLocation("misc/additionalmapicons.png", ResourceCategory.AdditionalMapIconsAtlas, 4),
        };

        public static string GetPathFromCategory(ResourceCategory category)
        {
            return category switch
            {
                ResourceCategory.ItemAnimation              => _categoryLookUp["textures/items"].ToString(),
                ResourceCategory.BlockAnimation             => _categoryLookUp["textures/blocks"].ToString(),
                ResourceCategory.MobEntityTextures          => _categoryLookUp["mob"].ToString(),
                ResourceCategory.ItemEntityTextures         => _categoryLookUp["item"].ToString(),
                ResourceCategory.BlockAtlas                 => _categoryLookUp["terrain.png"].ToString(),
                ResourceCategory.ItemAtlas                  => _categoryLookUp["items.png"].ToString(),
                ResourceCategory.ParticleAtlas              => _categoryLookUp["particles.png"].ToString(),
                ResourceCategory.BannerAtlas                => _categoryLookUp["item/banner/Banner_Atlas.png"].ToString(),
                ResourceCategory.PaintingAtlas              => _categoryLookUp["art/kz.png"].ToString(),
                ResourceCategory.ExplosionAtlas             => _categoryLookUp["misc/explosion.png"].ToString(),
                ResourceCategory.ExperienceOrbAtlas         => _categoryLookUp["item/xporb.png"].ToString(),
                ResourceCategory.MoonPhaseAtlas             => _categoryLookUp["terrain/moon_phases.png"].ToString(),
                ResourceCategory.MapIconAtlas               => _categoryLookUp["misc/mapicons.png"].ToString(),
                ResourceCategory.AdditionalMapIconsAtlas    => _categoryLookUp["misc/additionalmapicons.png"].ToString(),
                _ => string.Empty
            };
        }

        public static ResourceCategory GetCategoryFromPath(string path) => GetFromPath(path).Category;

        public static ResourceLocation GetFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("res/"))
                return Unknown;
            string categoryPath = path.Substring("res/".Length);
            if (_categoryLookUp.ContainsKey(categoryPath))
                return _categoryLookUp[categoryPath];
            return ResourceGroups.Where(group => categoryPath.StartsWith(group.Path)).FirstOrDefault() ?? Unknown;
        }

        public enum TillingMode
        { 
            Width,
            Height,
            WidthAndHeight
        }

        public readonly string Path;
        public readonly ResourceCategory Category;
        public readonly Size TillingFactor;
        public readonly TillingMode Tilling;
        public readonly bool IsGroup;

        public Size GetTileArea(Size imgSize)
        {
            int tileFactorWidth  = Math.Max(1, TillingFactor.Width);
            int tileFactorHeight = Math.Max(1, TillingFactor.Height);
            return Tilling switch
            {
                TillingMode.Width => new Size(imgSize.Width / tileFactorWidth, imgSize.Width / tileFactorHeight),
                TillingMode.Height => new Size(imgSize.Height / tileFactorWidth, imgSize.Height / tileFactorHeight),
                TillingMode.WidthAndHeight => new Size(imgSize.Width / tileFactorWidth, imgSize.Height / tileFactorHeight),
                _ => Size.Empty,
            };
        }

        private ResourceLocation(string path, ResourceCategory category, int tillingFactor, TillingMode tilling = TillingMode.Width, bool isGroup = false)
            : this(path, category, new Size(tillingFactor, tillingFactor), tilling, isGroup)
        {
        }

        private ResourceLocation(string path, ResourceCategory category, Size tillingFactor, TillingMode tilling = TillingMode.Width, bool isGroup = false)
        {
            Path = path;
            Category = category;
            TillingFactor = tillingFactor;
            Tilling = tilling;
            IsGroup = isGroup;
            if (isGroup)
                ResourceGroups.Add(this);
        }

        public override string ToString()
        {
            return "res/" + Path;
        }
    }
}
