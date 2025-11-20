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
using PckStudio.Core.Properties;
using PckStudio.Json;
using static PckStudio.Core.ResourceLocation;

namespace PckStudio.Core
{
    public static class ResourceLocations
    {
        static ResourceLocations()
        {
            _all = new ResourceLocation[] {
                    new GroupResource("textures/items", ResourceCategory.ItemAnimation),
                    new GroupResource("textures/blocks", ResourceCategory.BlockAnimation),
                    new GroupResource("mob", ResourceCategory.MobEntityTextures),
                    new GroupResource("item", ResourceCategory.ItemEntityTextures),
                    new GroupResource("armor", ResourceCategory.ArmorTextures),
                    new AtlasResource("terrain.png", ResourceCategory.BlockAtlas, 16, Resources.terrain_atlas, tilesInfo: Tiles.BlockTileInfos, atlasGroups: _terrainAtlasGroups),
                    new AtlasResource("items.png", ResourceCategory.ItemAtlas, 16, Resources.items_atlas, tilesInfo: Tiles.ItemTileInfos, atlasGroups: _itemsAtlasGroups),
                    new AtlasResource("particles.png", ResourceCategory.ParticleAtlas, 16, Resources.particles_atlas, tilesInfo: Tiles.ParticleTileInfos, atlasGroups: _particaleAtlasGroups),
                    new AtlasResource("item/banner/Banner_Atlas.png", ResourceCategory.BannerAtlas, new Size(6, 7), Resources.banners_atlas, TillingMode.WidthAndHeight, tilesInfo: Tiles.BannerTileInfos),
                    new AtlasResource("art/kz.png", ResourceCategory.PaintingAtlas, 16, Resources.paintings_atlas, tilesInfo: Tiles.PaintingTileInfos, atlasGroups: _paintingAtlasGroups),
                    new AtlasResource("misc/explosion.png", ResourceCategory.ExplosionAtlas, 16, Resources.explosions_atlas, tilesInfo: Tiles.ExplosionTileInfos),
                    new AtlasResource("item/xporb.png", ResourceCategory.ExperienceOrbAtlas, 4, Resources.experience_orbs_atlas, tilesInfo: Tiles.ExperienceOrbTileInfos),
                    new AtlasResource("terrain/moon_phases.png", ResourceCategory.MoonPhaseAtlas, 4, Resources.moon_phases_atlas, tilesInfo: Tiles.MoonPhaseTileInfos),
                    new AtlasResource("misc/mapicons.png", ResourceCategory.MapIconAtlas, 4, Resources.map_icons_atlas, tilesInfo: Tiles.MapIconTileInfos),
                    new AtlasResource("misc/additionalmapicons.png", ResourceCategory.AdditionalMapIconsAtlas, 4, Resources.additional_map_icons_atlas, tilesInfo: Tiles.AdditionalMapIconTileInfos),
                };
        }

        public static ResourceLocation GetFromCategory(ResourceCategory category) => ResourceLocation.GetFromCategory(category);
        public static ResourceLocation GetFromPath(string path) => ResourceLocation.GetFromPath(path);
        public static ResourceCategory GetCategoryFromPath(string path) => ResourceLocation.GetCategoryFromPath(path);
        public static string GetPathFromCategory(ResourceCategory category) => ResourceLocation.GetPathFromCategory(category);


        private static readonly AtlasGroup[] _particaleAtlasGroups =
        {
            new AtlasGroupAnimation("generic"            , row: 0, column:  0, frameCount:  8, ImageLayoutDirection.Horizontal, 2),
            new AtlasGroupAnimation("splash"             , row: 3, column:  1, frameCount:  4, ImageLayoutDirection.Horizontal, 2),
            new AtlasGroupAnimation("drip"               , row: 0, column:  7, frameCount:  3, ImageLayoutDirection.Horizontal, 4),
            new AtlasGroupAnimation("effect"             , row: 0, column:  8, frameCount:  8, ImageLayoutDirection.Horizontal, 2),
            new AtlasGroupAnimation("splash_effect"      , row: 0, column:  9, frameCount:  8, ImageLayoutDirection.Horizontal, 2),
            new AtlasGroupAnimation("firework_spark"     , row: 0, column: 10, frameCount:  8, ImageLayoutDirection.Horizontal, 2),
            new AtlasGroupAnimation("glitter"            , row: 0, column: 11, frameCount:  8, ImageLayoutDirection.Horizontal, 2),
            new AtlasGroupAnimation("BE_explosion"       , row: 0, column: 12, frameCount: 16, ImageLayoutDirection.Horizontal),
            new AtlasGroupLargeTile("flash"              , row: 4, column:  2, rowSpan: 4, columnSpan: 4),
            new AtlasGroupLargeTileAnimation("bubble_pop", row: 6, column:  6, rowSpan: 2, columnSpan: 2, frameCount: 5, ImageLayoutDirection.Horizontal, 2),
        };

        private static readonly AtlasGroup[] _terrainAtlasGroups =
        {
             new AtlasGroupLargeTile("Oak Door"     , row: 1, column:  5, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Iron Door"    , row: 2, column:  5, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Acacia Door"  , row: 0, column: 23, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Birch Door"   , row: 1, column: 23, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Dark Oak Door", row: 2, column: 23, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Jungle Door"  , row: 3, column: 23, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Spruce Door"  , row: 4, column: 23, rowSpan: 1, columnSpan: 2),

             new AtlasGroupLargeTile("Large Fern"       , row: 0, column: 20, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Double Tall Grass", row: 1, column: 20, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Poeny"            , row: 2, column: 20, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Rose Bush"        , row: 3, column: 20, rowSpan: 1, columnSpan: 2),
             new AtlasGroupLargeTile("Lilac"            , row: 4, column: 20, rowSpan: 1, columnSpan: 2),

             new AtlasGroupAnimation("Wheat"      , row: 8, column:  5, frameCount:  8, ImageLayoutDirection.Horizontal, 5),
             new AtlasGroupAnimation("Potatoes"   , row: 9, column: 16, frameCount:  4, ImageLayoutDirection.Horizontal, 5),
             new AtlasGroupAnimation("Carrots"    , row: 8, column: 12, frameCount:  4, ImageLayoutDirection.Horizontal, 5),
             new AtlasGroupAnimation("Beetroots"  , row: 0, column: 25, frameCount:  4, ImageLayoutDirection.Horizontal, 5),
             new AtlasGroupAnimation("Nether Wart", row: 2, column: 14, frameCount:  3, ImageLayoutDirection.Horizontal, 5),
             new AtlasGroupAnimation("Destroy"    , row: 0, column: 15, frameCount: 10, ImageLayoutDirection.Horizontal, 3),
        };

        private static readonly AtlasGroup[] _itemsAtlasGroups =
        {
            new AtlasGroupAnimation("Bow Pulling", row: 5, column: 6, frameCount: 3, ImageLayoutDirection.Vertical, 6),
        };

        private static readonly AtlasGroup[] _paintingAtlasGroups =
        {
            new AtlasGroupLargeTile("The Pool"                , row:  0, column: 2, rowSpan: 2, columnSpan: 1),
            new AtlasGroupLargeTile("Bonjour Monsiuer Courbet", row:  2, column: 2, rowSpan: 2, columnSpan: 1),
            new AtlasGroupLargeTile("Seaside"                 , row:  4, column: 2, rowSpan: 2, columnSpan: 1),
            new AtlasGroupLargeTile("sunset_dense"            , row:  6, column: 2, rowSpan: 2, columnSpan: 1),
            new AtlasGroupLargeTile("Creebet"                 , row:  8, column: 2, rowSpan: 2, columnSpan: 1),

            new AtlasGroupLargeTile("Wanderer"                , row:  0, column: 4, rowSpan: 1, columnSpan: 2),
            new AtlasGroupLargeTile("Graham"                  , row:  1, column: 4, rowSpan: 1, columnSpan: 2),

            new AtlasGroupLargeTile("Fighters"                , row:  0, column: 6, rowSpan: 4, columnSpan: 2),

            new AtlasGroupLargeTile("Match"                   , row:  0, column: 8, rowSpan: 2, columnSpan: 2),
            new AtlasGroupLargeTile("Bust"                    , row:  2, column: 8, rowSpan: 2, columnSpan: 2),
            new AtlasGroupLargeTile("The stage is set"        , row:  4, column: 8, rowSpan: 2, columnSpan: 2),
            new AtlasGroupLargeTile("The Void"                , row:  6, column: 8, rowSpan: 2, columnSpan: 2),
            new AtlasGroupLargeTile("Skull and Roses"         , row:  8, column: 8, rowSpan: 2, columnSpan: 2),
            new AtlasGroupLargeTile("Wither"                  , row: 10, column: 8, rowSpan: 2, columnSpan: 2),

            new AtlasGroupLargeTile("Mortal Coil"             , row: 12, column: 4, rowSpan: 4, columnSpan: 3),
            new AtlasGroupLargeTile("Kong"                    , row: 12, column: 7, rowSpan: 4, columnSpan: 3),

            new AtlasGroupLargeTile("Back Texture"            , row: 12, column: 0, rowSpan: 4, columnSpan: 4),
            new AtlasGroupLargeTile("Pointer"                 , row:  0, column: 12, rowSpan: 4, columnSpan: 4),
            new AtlasGroupLargeTile("Pigscene"                , row:  4, column: 12, rowSpan: 4, columnSpan: 4),
            new AtlasGroupLargeTile("Skull On Fire"           , row:  8, column: 12, rowSpan: 4, columnSpan: 4),
        };

        private static readonly ResourceLocation[] _all;
    }

    public class ResourceLocation
    {
        internal const string RESOURCE_PATH_PREFIX = "res/";
        private static List<ResourceLocation> ResourceGroups = new List<ResourceLocation>();
        private static readonly ResourceLocation Unknown = new ResourceLocation(string.Empty, ResourceCategory.Unknown, -1);
        private static readonly Dictionary<string, ResourceLocation> _pathLookUp = new Dictionary<string, ResourceLocation>();
        private static readonly Dictionary<ResourceCategory, ResourceLocation> _categoryLookUp = new Dictionary<ResourceCategory, ResourceLocation>();

        public enum TillingMode
        {
            Width,
            Height,
            WidthAndHeight
        }

        public string Path { get; }
        public string FullPath => System.IO.Path.Combine(RESOURCE_PATH_PREFIX, Path);
        public ResourceCategory Category { get; }
        public Size TillingFactor { get; }
        public TillingMode Tilling { get; }
        public bool IsGroup { get; }

        public Size GetTileArea(Size imgSize)
        {
            return Tilling switch
            {
                TillingMode.Width => new Size(imgSize.Width / TillingFactor.Width, imgSize.Width / TillingFactor.Height),
                TillingMode.Height => new Size(imgSize.Height / TillingFactor.Width, imgSize.Height / TillingFactor.Height),
                TillingMode.WidthAndHeight => new Size(imgSize.Width / TillingFactor.Width, imgSize.Height / TillingFactor.Height),
                _ => Size.Empty,
            };
        }

        protected ResourceLocation(string path, ResourceCategory category, int tillingFactor, TillingMode tilling = default, bool isGroup = false)
            : this(path, category, new Size(tillingFactor, tillingFactor), tilling, isGroup)
        {
        }


        protected ResourceLocation(string path, ResourceCategory category, Size tillingFactor, TillingMode tilling = default, bool isGroup = false)
        {
            Path = path;
            Category = category;
            TillingFactor = new Size(Math.Max(1, tillingFactor.Width), Math.Max(1, tillingFactor.Height));
            Tilling = Enum.IsDefined(typeof(TillingMode), tilling) ? tilling : default;
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
