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
using System.IO;
using System.Linq;
using PckStudio.Core.Json;

namespace PckStudio.Core
{
    public sealed class AtlasResource : ResourceLocation
    {
        public enum TillingMode
        {
            X,
            Y,
            XY
        }

        public enum AtlasType
        {
            Invalid,
            ItemAtlas,
            BlockAtlas,
            ParticleAtlas,
            BannerAtlas,
            PaintingAtlas,
            ExplosionAtlas,
            ExperienceOrbAtlas,
            MoonPhaseAtlas,
            MapIconAtlas,
            AdditionalMapIconsAtlas,
        }

        public IEnumerable<JsonTileInfo> TilesInfo { get; }
        public AtlasGroup[] AtlasGroups { get; }
        public readonly Image DefaultTexture;
        public Size TileCount { get; }
        public TillingMode TileCountAxis { get; }
        public AtlasType Type { get; }

        public AtlasResource(string path, AtlasType atlasType, int tileCount, Image defaultTexture, TillingMode tilling = default, IEnumerable<JsonTileInfo> tilesInfo = default, AtlasGroup[] atlasGroups = default)
            : this(path, atlasType, new Size(tileCount, tileCount), defaultTexture, tilling, tilesInfo, atlasGroups)
        {
        }

        public AtlasResource(string path, AtlasType atlasType, Size tillingFactor, Image defaultTexture, TillingMode tilling = default, IEnumerable<JsonTileInfo> tilesInfo = default, AtlasGroup[] atlasGroups = default)
            : base(path, GetId(atlasType), isGroup: false)
        {
            TilesInfo = tilesInfo ?? Enumerable.Empty<JsonTileInfo>();
            AtlasGroups = atlasGroups ?? Array.Empty<AtlasGroup>();
            DefaultTexture = defaultTexture;
            TileCount = new Size(Math.Max(1, tillingFactor.Width), Math.Max(1, tillingFactor.Height));
            TileCountAxis = Enum.IsDefined(typeof(TillingMode), tilling) ? tilling : default;
            Type = atlasType;
        }

        public Atlas GetDefaultAtlas()
        {
            return Atlas.FromResourceLocation(DefaultTexture, this);
        }

        public Size GetTileArea(Size imgSize)
        {
            return TileCountAxis switch
            {
                TillingMode.X => new Size(imgSize.Width / TileCount.Width, imgSize.Width / TileCount.Height),
                TillingMode.Y => new Size(imgSize.Height / TileCount.Width, imgSize.Height / TileCount.Height),
                TillingMode.XY => new Size(imgSize.Width / TileCount.Width, imgSize.Height / TileCount.Height),
                _ => Size.Empty,
            };
        }

        public static ResourceCategory GetId(AtlasType atlasType) => (ResourceCategory)((int)ResourceCategory.Atlas | (int)atlasType);
    }
}
