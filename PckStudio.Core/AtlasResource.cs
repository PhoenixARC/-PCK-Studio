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
using PckStudio.Core.Deserializer;
using PckStudio.Core.Json;

namespace PckStudio.Core
{
    public sealed class AtlasResource : ResourceLocation
    {
        public IEnumerable<JsonTileInfo> TilesInfo { get; }
        public AtlasGroup[] AtlasGroups { get; }
        public readonly Image DefaultTexture;

        public AtlasResource(string path, ResourceCategory resourceCategory, int tillingFactor, Image defaultTexture, TillingMode tilling = default, IEnumerable<JsonTileInfo> tilesInfo = default, AtlasGroup[] atlasGroups = default)
            : this(path, resourceCategory, new Size(tillingFactor, tillingFactor), defaultTexture, tilling, tilesInfo, atlasGroups)
        {
        }

        public AtlasResource(string path, ResourceCategory resourceCategory, Size tillingFactor, Image defaultTexture, TillingMode tilling = default, IEnumerable<JsonTileInfo> tilesInfo = default, AtlasGroup[] atlasGroups = default)
            : base(path, resourceCategory, tillingFactor, tilling, isGroup: true)
        {
            TilesInfo = tilesInfo ?? Enumerable.Empty<JsonTileInfo>();
            AtlasGroups = atlasGroups ?? Array.Empty<AtlasGroup>();
            DefaultTexture = defaultTexture;
        }

        public Atlas GetDefaultAtlas()
        {
            return Atlas.FromResourceLocation(DefaultTexture, this);
        }
    }
}
