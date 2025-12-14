/* Copyright (c) 2025-present miku-666
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
using System.Drawing;
using Newtonsoft.Json;

namespace PckStudio.Core
{
    public abstract class AtlasGroup
    {
        [JsonProperty("name", Required = Required.Always, Order = 0)]
        public string Name { get; set; }
        [JsonProperty("row", Required = Required.Always, Order = 1)]
        public int Row { get; }
        [JsonProperty("column", Required = Required.Always, Order = 2)]
        public int Column { get; }
        [JsonProperty("allowCustomColor", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowCustomColor { get; }

        [JsonIgnore]
        public virtual int Count => 1;

        [JsonIgnore]
        protected abstract bool isAnimation { get; }
        [JsonIgnore]
        protected abstract bool isLargeTile { get; }
        [JsonIgnore]
        protected abstract bool isComposedOfMultipleTiles { get; }
        [JsonProperty("direction")]
        public virtual ImageLayoutDirection Direction => Column > Row ? ImageLayoutDirection.Vertical : ImageLayoutDirection.Horizontal;

        public AtlasGroup(string name, int row, int column, bool allowCustomColor)
        {
            Name = name;
            Row = Math.Max(0, row);
            Column = Math.Max(0, column);
            AllowCustomColor = allowCustomColor;
        }

        public bool IsAnimation() => isAnimation;
        public bool IsLargeTile() => isLargeTile;
        public bool IsComposedOfMultipleTiles() => isComposedOfMultipleTiles;

        public abstract Size GetSize(Size tileSize);

        internal virtual Rectangle[] GetTileArea(Size tileSize)
        {
            return [new Rectangle(new Point(Row * tileSize.Width, Column * tileSize.Height), GetSize(tileSize))];
        }
    }
}