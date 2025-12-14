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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PckStudio.Core.Properties;
using PckStudio.Json;

namespace PckStudio.Core
{
    public static class ResourceLocations
    {
        static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            Converters = [new AtlasGroupJsonConverter()]
        };
        static ResourceLocations()
        {
            _all = new ResourceLocation[] {
                    new GroupResource("textures/items", ResourceCategory.ItemAnimation),
                    new GroupResource("textures/blocks", ResourceCategory.BlockAnimation),
                    new GroupResource("mob", ResourceCategory.MobEntityTextures),
                    new GroupResource("item", ResourceCategory.ItemEntityTextures),
                    new GroupResource("armor", ResourceCategory.ArmorTextures),
                    new AtlasResource("terrain.png", AtlasResource.AtlasType.BlockAtlas, 16, Resources.terrain_atlas, tilesInfo: Tiles.BlockTileInfos, atlasGroups: _terrainAtlasGroups),
                    new AtlasResource("items.png", AtlasResource.AtlasType.ItemAtlas, 16, Resources.items_atlas, tilesInfo: Tiles.ItemTileInfos, atlasGroups: _itemsAtlasGroups),
                    new AtlasResource("particles.png", AtlasResource.AtlasType.ParticleAtlas, 16, Resources.particles_atlas, tilesInfo: Tiles.ParticleTileInfos, atlasGroups: _particaleAtlasGroups),
                    new AtlasResource("item/banner/Banner_Atlas.png", AtlasResource.AtlasType.BannerAtlas, new Size(6, 7), Resources.banners_atlas, AtlasResource.TillingMode.XY, tilesInfo: Tiles.BannerTileInfos),
                    new AtlasResource("art/kz.png", AtlasResource.AtlasType.PaintingAtlas, 16, Resources.paintings_atlas, tilesInfo: Tiles.PaintingTileInfos, atlasGroups: _paintingAtlasGroups),
                    new AtlasResource("misc/explosion.png", AtlasResource.AtlasType.ExplosionAtlas, 16, Resources.explosions_atlas, tilesInfo: Tiles.ExplosionTileInfos),
                    new AtlasResource("item/xporb.png", AtlasResource.AtlasType.ExperienceOrbAtlas, 4, Resources.experience_orbs_atlas, tilesInfo: Tiles.ExperienceOrbTileInfos),
                    new AtlasResource("terrain/moon_phases.png", AtlasResource.AtlasType.MoonPhaseAtlas, 4, Resources.moon_phases_atlas, tilesInfo: Tiles.MoonPhaseTileInfos),
                    new AtlasResource("misc/mapicons.png", AtlasResource.AtlasType.MapIconAtlas, 4, Resources.map_icons_atlas, tilesInfo: Tiles.MapIconTileInfos),
                    new AtlasResource("misc/additionalmapicons.png", AtlasResource.AtlasType.AdditionalMapIconsAtlas, 4, Resources.additional_map_icons_atlas, tilesInfo: Tiles.AdditionalMapIconTileInfos),
                };
        }

        public static ResourceLocation GetFromCategory(ResourceCategory category) => ResourceLocation.GetFromCategory(category);
        public static ResourceLocation GetFromPath(string path) => ResourceLocation.GetFromPath(path);
        public static ResourceCategory GetCategoryFromPath(string path) => ResourceLocation.GetCategoryFromPath(path);
        public static string GetPathFromCategory(ResourceCategory category) => ResourceLocation.GetPathFromCategory(category);


        private static readonly AtlasGroup[] _particaleAtlasGroups = JsonConvert.DeserializeObject<AtlasGroup[]>(Resources.particles_groups, _serializerSettings);

        private static readonly AtlasGroup[] _terrainAtlasGroups = JsonConvert.DeserializeObject<AtlasGroup[]>(Resources.terrain_groups, _serializerSettings);

        private static readonly AtlasGroup[] _itemsAtlasGroups = JsonConvert.DeserializeObject<AtlasGroup[]>(Resources.items_groups, _serializerSettings);

        private static readonly AtlasGroup[] _paintingAtlasGroups = JsonConvert.DeserializeObject<AtlasGroup[]>(Resources.painting_groups, _serializerSettings);
        

        private static readonly ResourceLocation[] _all;
    }
}
