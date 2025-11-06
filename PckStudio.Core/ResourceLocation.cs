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
using PckStudio.Core.Json;
using PckStudio.Json;

namespace PckStudio.Core
{
    public sealed class ResourceLocation
    {
        private static List<ResourceLocation> ResourceGroups = new List<ResourceLocation>();
        private static readonly ResourceLocation Unknown = new ResourceLocation(string.Empty, ResourceCategory.Unknown, -1);

        private static AtlasGroup[] _particaleAtlasGroups =
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

        private static AtlasGroup[] _terrainAtlasGroups =
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

        private static AtlasGroup[] _itemsAtlasGroups =
        {
            new AtlasGroupAnimation("Bow Pulling", row: 5, column: 6, frameCount: 3, ImageLayoutDirection.Vertical, 6),
        };

        private static AtlasGroup[] _paintingAtlasGroups =
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

        private static readonly Dictionary<string, ResourceLocation> _categoryLookUp = new Dictionary<string, ResourceLocation>()
        {
            ["textures/items"] = new ResourceLocation("textures/items", ResourceCategory.ItemAnimation, 16, isGroup: true),
            ["textures/blocks"] = new ResourceLocation("textures/blocks", ResourceCategory.BlockAnimation, 16, isGroup: true),
            ["mob"] = new ResourceLocation("mob", ResourceCategory.MobEntityTextures, 1, isGroup: true),
            ["item"] = new ResourceLocation("item", ResourceCategory.ItemEntityTextures, 1, isGroup: true),
            ["terrain.png"] = new ResourceLocation("terrain.png", ResourceCategory.BlockAtlas, 16, tilesInfo: Tiles.BlockTileInfos, atlasGroups: _terrainAtlasGroups),
            ["items.png"] = new ResourceLocation("items.png", ResourceCategory.ItemAtlas, 16, tilesInfo: Tiles.ItemTileInfos, atlasGroups: _itemsAtlasGroups),
            ["particles.png"] = new ResourceLocation("particles.png", ResourceCategory.ParticleAtlas, 16, tilesInfo: Tiles.ParticleTileInfos, atlasGroups: _particaleAtlasGroups),
            //============ TODO ============//
            ["item/banner/Banner_Atlas.png"] = new ResourceLocation("item/banner/Banner_Atlas.png", ResourceCategory.BannerAtlas, new Size(6, 7), TillingMode.WidthAndHeight, tilesInfo: Tiles.BannerTileInfos),
            //==============================//
            ["art/kz.png"] = new ResourceLocation("art/kz.png", ResourceCategory.PaintingAtlas, 16, tilesInfo: Tiles.PaintingTileInfos, atlasGroups: _paintingAtlasGroups),
            ["misc/explosion.png"] = new ResourceLocation("misc/explosion.png", ResourceCategory.ExplosionAtlas, 16, tilesInfo: Tiles.ExplosionTileInfos),
            ["item/xporb.png"] = new ResourceLocation("item/xporb.png", ResourceCategory.ExperienceOrbAtlas, 4, tilesInfo: Tiles.ExperienceOrbTileInfos),
            ["terrain/moon_phases.png"] = new ResourceLocation("terrain/moon_phases.png", ResourceCategory.MoonPhaseAtlas, 4, tilesInfo: Tiles.MoonPhaseTileInfos),
            ["misc/mapicons.png"] = new ResourceLocation("misc/mapicons.png", ResourceCategory.MapIconAtlas, 4, tilesInfo: Tiles.MapIconTileInfos),
            ["misc/additionalmapicons.png"] = new ResourceLocation("misc/additionalmapicons.png", ResourceCategory.AdditionalMapIconsAtlas, 4, tilesInfo: Tiles.AdditionalMapIconTileInfos),
        };

        public static string GetPathFromCategory(ResourceCategory category)
        {
            return category switch
            {
                ResourceCategory.ItemAnimation => _categoryLookUp["textures/items"].ToString(),
                ResourceCategory.BlockAnimation => _categoryLookUp["textures/blocks"].ToString(),
                ResourceCategory.MobEntityTextures => _categoryLookUp["mob"].ToString(),
                ResourceCategory.ItemEntityTextures => _categoryLookUp["item"].ToString(),
                ResourceCategory.BlockAtlas => _categoryLookUp["terrain.png"].ToString(),
                ResourceCategory.ItemAtlas => _categoryLookUp["items.png"].ToString(),
                ResourceCategory.ParticleAtlas => _categoryLookUp["particles.png"].ToString(),
                ResourceCategory.BannerAtlas => _categoryLookUp["item/banner/Banner_Atlas.png"].ToString(),
                ResourceCategory.PaintingAtlas => _categoryLookUp["art/kz.png"].ToString(),
                ResourceCategory.ExplosionAtlas => _categoryLookUp["misc/explosion.png"].ToString(),
                ResourceCategory.ExperienceOrbAtlas => _categoryLookUp["item/xporb.png"].ToString(),
                ResourceCategory.MoonPhaseAtlas => _categoryLookUp["terrain/moon_phases.png"].ToString(),
                ResourceCategory.MapIconAtlas => _categoryLookUp["misc/mapicons.png"].ToString(),
                ResourceCategory.AdditionalMapIconsAtlas => _categoryLookUp["misc/additionalmapicons.png"].ToString(),
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

        public string Path { get; }
        public ResourceCategory Category { get; }
        public Size TillingFactor { get; }
        public TillingMode Tilling { get; }
        public bool IsGroup { get; }
        public IEnumerable<JsonTileInfo> TilesInfo { get; }
        public IEnumerable<AtlasGroup> AtlasGroups { get; }

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

        private ResourceLocation(string path, ResourceCategory category, int tillingFactor, TillingMode tilling = TillingMode.Width, bool isGroup = false, IEnumerable<JsonTileInfo> tilesInfo = default, IEnumerable<AtlasGroup> atlasGroups = default)
            : this(path, category, new Size(tillingFactor, tillingFactor), tilling, isGroup, tilesInfo, atlasGroups)
        {
        }


        private ResourceLocation(string path, ResourceCategory category, Size tillingFactor, TillingMode tilling = TillingMode.Width, bool isGroup = false, IEnumerable<JsonTileInfo> tilesInfo = default, IEnumerable<AtlasGroup> atlasGroups = default)
        {
            Path = path;
            Category = category;
            TillingFactor = new Size(Math.Max(1, tillingFactor.Width), Math.Max(1, tillingFactor.Height));
            Tilling = tilling;
            IsGroup = isGroup;
            TilesInfo = tilesInfo ?? Enumerable.Empty<JsonTileInfo>();
            AtlasGroups = atlasGroups ?? Enumerable.Empty<AtlasGroup>();
            if (isGroup)
                ResourceGroups.Add(this);
        }

        public override string ToString()
        {
            return "res/" + Path;
        }
    }
}
