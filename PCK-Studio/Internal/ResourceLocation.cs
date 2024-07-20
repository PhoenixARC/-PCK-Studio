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

namespace PckStudio.Internal
{
    internal sealed class ResourceLocation
    {
        private static readonly Dictionary<string, ResourceLocation> _categoryLookUp = new Dictionary<string, ResourceLocation>()
        {
            ["textures/items"]               = new ResourceLocation("textures/items", ResourceCategory.ItemAnimation, 16, isGroup: true),
            ["textures/blocks"]              = new ResourceLocation("textures/blocks", ResourceCategory.BlockAnimation, 16, isGroup: true),
            ["terrain.png"]                  = new ResourceLocation("terrain.png", ResourceCategory.BlockAtlas, 16),
            ["items.png"]                    = new ResourceLocation("items.png", ResourceCategory.ItemAtlas, 16),
            ["particles.png"]                = new ResourceLocation("particles.png", ResourceCategory.ParticleAtlas, 16),
            ["item/banner/Banner_Atlas.png"] = new ResourceLocation("item/banner/Banner_Atlas.png", ResourceCategory.BannerAtlas, new Size(6, 7), TillingMode.Custom),
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
                ResourceCategory.ItemAnimation              => "res/textures/items",
                ResourceCategory.BlockAnimation             => "res/textures/blocks",
                ResourceCategory.BlockAtlas                 => "res/terrain.png",
                ResourceCategory.ItemAtlas                  => "res/items.png",
                ResourceCategory.ParticleAtlas              => "res/particles.png",
                ResourceCategory.BannerAtlas                => "res/item/banner/Banner_Atlas.png",
                ResourceCategory.PaintingAtlas              => "res/art/kz.png",
                ResourceCategory.ExplosionAtlas             => "res/misc/explosion.png",
                ResourceCategory.ExperienceOrbAtlas         => "res/item/xporb.png",
                ResourceCategory.MoonPhaseAtlas             => "res/terrain/moon_phases.png",
                ResourceCategory.MapIconAtlas               => "res/misc/mapicons.png",
                ResourceCategory.AdditionalMapIconsAtlas    => "res/misc/additionalmapicons.png",
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

            string categoryPath = path.Substring("res/".Length);
            return _categoryLookUp.ContainsKey(categoryPath) ? _categoryLookUp[categoryPath].Category : ResourceCategory.Unknown;
        }

        public static ResourceLocation GetFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("res/"))
                return null;
            string categoryPath = path.Substring("res/".Length);
            if (categoryPath.StartsWith("textures/items")) 
                categoryPath = "textures/items";
            if (categoryPath.StartsWith("textures/blocks"))
                categoryPath = "textures/blocks";
            return _categoryLookUp.ContainsKey(categoryPath) ? _categoryLookUp[categoryPath] : null;
        }

        public enum TillingMode
        { 
            Width,
            Height,
            Custom
        }

        public readonly string Path;
        public readonly ResourceCategory Category;
        public readonly Size TillingFactor;
        public readonly TillingMode TillingResolution;
        public readonly bool IsGroup;

        public Size GetTileArea(Size imgSize)
        {
            int tileFactorWidth  = Math.Max(1, TillingFactor.Width);
            int tileFactorHeight = Math.Max(1, TillingFactor.Height);
            return TillingResolution switch
            {
                TillingMode.Width => new Size(imgSize.Width / tileFactorWidth, imgSize.Width / tileFactorHeight),
                TillingMode.Height => new Size(imgSize.Height / tileFactorWidth, imgSize.Height / tileFactorHeight),
                TillingMode.Custom => new Size(imgSize.Width / tileFactorWidth, imgSize.Height / tileFactorHeight),
                _ => Size.Empty,
            };
        }

        private ResourceLocation(string path, ResourceCategory category, int tillingFactor, TillingMode tillingResolution = TillingMode.Width, bool isGroup = false)
            : this(path, category, new Size(tillingFactor, tillingFactor), tillingResolution, isGroup)
        {
        }

        private ResourceLocation(string path, ResourceCategory category, Size tillingFactor, TillingMode tillingResolution = TillingMode.Width, bool isGroup = false)
        {
            Path = path;
            Category = category;
            TillingFactor = tillingFactor;
            TillingResolution = tillingResolution;
            IsGroup = isGroup;
        }

        public override string ToString()
        {
            return "res/" + Path;
        }
    }
}
